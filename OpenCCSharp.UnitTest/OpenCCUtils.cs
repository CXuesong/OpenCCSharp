using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using O9d.Json.Formatting;
using OpenCCSharp.Conversion;

namespace OpenCCSharp.UnitTest;

internal static class OpenCCUtils
{

    public static readonly string OpenCCConversionConfigDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/config");

    public static readonly string OpenCCDictionaryDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/dictionary");

    public static readonly string OpenCCTestCasesDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/testcases");

    // <string, Task<SortedStringPrefixDictionary> | SortedStringPrefixDictionary>
    private static readonly ConcurrentDictionary<string, object> dictCache = new();

    public static IEnumerable<(string Input, string Output)> ReadTestCases(string caseSetName)
    {
        return File.ReadLines(Path.Join(OpenCCTestCasesDir, caseSetName + ".in"))
            .Zip(File.ReadLines(Path.Join(OpenCCTestCasesDir, caseSetName + ".ans")));
    }

    public static async ValueTask<SortedStringPrefixDictionary> CreateDictionaryFromAsync(string dictFileName)
    {
        var dict = await GetDictionaryFromAsync(dictFileName);
        return new SortedStringPrefixDictionary(dict);
    }

    public static ValueTask<SortedStringPrefixDictionary> GetDictionaryFromAsync(string dictFileName)
    {
        static async Task<SortedStringPrefixDictionary> ValueFactory(string fn, ConcurrentDictionary<string, object> dc)
        {
            await Task.Yield();
            var dict = new SortedStringPrefixDictionary();
            var segments = fn.Split('|');

            var loadOptions = PlainTextConversionLookupTableLoadOptions.None;
            if (segments.Length > 1 && segments[1].Equals("Reverse", StringComparison.OrdinalIgnoreCase))
                loadOptions |= PlainTextConversionLookupTableLoadOptions.ReverseEntries;

            await PlainTextConversionLookupTable.LoadAsync(dict, Path.Join(OpenCCDictionaryDir, segments[0]), loadOptions);
            dc[fn] = dict;
            return dict;
        }

        return dictCache.GetOrAdd(dictFileName, static (fn, dc) => ValueFactory(fn, dc), dictCache) switch
        {
            Task<SortedStringPrefixDictionary> t => new(t),
            SortedStringPrefixDictionary d => new(d)
        };
    }

    public static async Task<ScriptConverter> CreateConverterFromAsync(string configFileName)
    {
        ConversionConfigRoot config;
        await using (var fs = File.OpenRead(Path.Join(OpenCCConversionConfigDir, configFileName)))
            config = JsonSerializer.Deserialize<ConversionConfigRoot>(fs, new JsonSerializerOptions
                     {
                         PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
                         ReadCommentHandling = JsonCommentHandling.Skip
                     })
                     ?? throw new InvalidOperationException("Config JSON resolves to null.");
        var lexer = await config.Segmentation.ResolveAsync();
        var conversionChain = await Utility.WhenAll(config.ConversionChain.Select(c => c.Dict.ResolveAsync()));
        return new ScriptConverter(lexer, new ChainedStringMapping((IEnumerable<IStringPrefixMapping>)conversionChain));
    }

    #region Contracts

    private sealed class ConversionConfigRoot
    {
        public string Name { get; set; }
        public Segmentation Segmentation { get; set; }
        public List<ConversionChainItem> ConversionChain { get; set; }
    }

    private sealed class ConversionChainItem
    {
        public Dict Dict { get; set; }
    }

    private sealed class Dict
    {

        // Some dictionaries are merged from others: https://github.com/BYVoid/OpenCC/blob/6ee1fc7e7993162c2038b423f6db31f3d6e20f30/node/dicts.gypi
        private static readonly Dictionary<string, string[]> knownMergedDicts = new()
        {
            { "TWPhrases.ocd2", new[] { "TWPhrasesIT.txt", "TWPhrasesName.txt", "TWPhrasesOther.txt" } }
        };

        public string Type { get; set; }
        public List<Dict> Dicts { get; set; }
        public string File { get; set; }

        public async ValueTask<IStringPrefixMapping> ResolveAsync()
        {
            if (!string.IsNullOrEmpty(File) && knownMergedDicts.TryGetValue(File, out var dicts))
                return await new Dict
                {
                    Type = "group",
                    Dicts = dicts.Select(fn => new Dict { Type = "text", File = fn }).ToList()
                }.ResolveAsync();
            switch (Type)
            {
                case "group":
                    var children = await Utility.WhenAll(Dicts.Select(d => d.ResolveAsync()));
                    return children.Length == 1 ? children[0] : new MergedStringPrefixMapping(children);
                case "ocd2":
                    // No support for ocd yet.
                    return await GetDictionaryFromAsync(Path.ChangeExtension(File, "txt"));
                case "text":
                    return await GetDictionaryFromAsync(File);
                default:
                    throw new InvalidOperationException($"Unknown dictionary type {Type}.");
            }
        }
    }

    private sealed class Segmentation
    {
        public string Type { get; set; }
        public Dict Dict { get; set; }

        public async ValueTask<IScriptLexer> ResolveAsync()
        {
            switch (Type)
            {
                case "mmseg":
                    var lexerDict = await Dict.ResolveAsync();
                    return new LongestPrefixLexer(lexerDict);
                default:
                    throw new InvalidOperationException($"Unknown segmentation type {Type}.");
            }
        }
    }

    #endregion

}
