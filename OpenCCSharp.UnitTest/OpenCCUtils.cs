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

    // <string, Task<SortedStringPrefixDictionary> | SortedStringPrefixDictionary>
    private static readonly ConcurrentDictionary<string, object> dictCache = new();

    public static async ValueTask<SortedStringPrefixDictionary> CreateDictionaryFromAsync(string dictFileName)
    {
        var dict = await GetDictionaryFromAsync(dictFileName);
        return new SortedStringPrefixDictionary(dict);
    }

    private static ValueTask<SortedStringPrefixDictionary> GetDictionaryFromAsync(string dictFileName)
    {
        static async Task<SortedStringPrefixDictionary> ValueFactory(string fn, ConcurrentDictionary<string, object> dc)
        {
            await Task.Yield();
            var dict = new SortedStringPrefixDictionary();
            await PlainTextConversionLookupTable.Load(dict, Path.Join(OpenCCDictionaryDir, fn));
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
                         PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
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
        public string Type { get; set; }
        public List<Dict> Dicts { get; set; }
        public string File { get; set; }

        public async ValueTask<IStringPrefixMapping> ResolveAsync()
        {
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
