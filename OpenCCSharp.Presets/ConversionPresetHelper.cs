using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenCCSharp.Conversion;

namespace OpenCCSharp.Presets;

internal static class ConversionPresetHelper
{

    private static readonly string OpenCCDictionariesNamespacePrefix = typeof(ConversionPresetHelper).Namespace + ".ConversionDictionaries.";

    private static readonly string ConversionDefinitionsNamespacePrefix = typeof(ConversionPresetHelper).Namespace + ".ConversionDefinitions.";

    // <string, Task<SortedStringPrefixDictionary> | SortedStringPrefixDictionary>
    private static readonly ConcurrentDictionary<string, object> dictCache = new();

    public static ValueTask<TrieStringPrefixDictionary> GetDictionaryFromAsync(string dictFileName)
    {
        static async Task<TrieStringPrefixDictionary> ValueFactory(string fn, ConcurrentDictionary<string, object> dc)
        {
            await Task.Yield();
            var dict = new TrieStringPrefixDictionary();
            var segments = fn.Split('|');

            var reverse = segments.Length > 1 && segments[1].Equals("Reverse", StringComparison.OrdinalIgnoreCase);

            var resourceName = OpenCCDictionariesNamespacePrefix + segments[0];
            await using (var rs = typeof(ConversionPresetHelper).Assembly.GetManifestResourceStream(resourceName))
            {
                if (rs == null)
                    throw new MissingManifestResourceException($"Failed to retrieve the manifest resource: {resourceName}.");

                await foreach (var (k, v) in PlainTextConversionLookupTable.EnumEntriesFromAsync(rs))
                {
                    if (reverse)
                    {
                        foreach (var v1 in v) dict.TryAdd(v1, k.ToArray());
                    }
                    else
                    {
                        dict.TryAdd(k, v[0].ToArray());
                    }
                }
            }

            dict.TrimExcess();
            dc[fn] = dict;
            return dict;
        }

        return dictCache.GetOrAdd(dictFileName, static (fn, dc) => ValueFactory(fn, dc), dictCache) switch
        {
            Task<TrieStringPrefixDictionary> t => new(t),
            TrieStringPrefixDictionary d => new(d),
            _ => throw new InvalidOperationException(),
        };
    }

    public static async ValueTask<ChainedScriptConverter> CreateConverterFromAsync(string configFileName)
    {
        ConversionDefinitionRoot config;
        var resourceName = ConversionDefinitionsNamespacePrefix + configFileName;
        await using (var rs = typeof(ConversionPresetHelper).Assembly.GetManifestResourceStream(resourceName))
        {
            if (rs == null)
                throw new MissingManifestResourceException($"Failed to retrieve the manifest resource: {resourceName}.");
            config = JsonSerializer.Deserialize<ConversionDefinitionRoot>(rs, new JsonSerializerOptions
                     {
                         ReadCommentHandling = JsonCommentHandling.Skip,
                     })
                     ?? throw new InvalidOperationException("Config JSON resolves to null.");
        }
        var converters = await config.ConversionSteps.SelectAsync(async step =>
        {
            var dicts = await step.Dictionaries
                .SelectAsync(GetDictionaryFromAsync)
                .ToListAsync();
            var mergedMapping = new MergedStringPrefixMapping(dicts);
            var lexer = new LongestPrefixLexer(mergedMapping);
            return new ScriptConverter(lexer, mergedMapping);
        }).ToListAsync();
        return new ChainedScriptConverter(converters);
    }

    public static void ClearCache()
    {
        dictCache.Clear();
    }

    private sealed class ConversionStep
    {
        public List<string> Dictionaries { get; set; } = default!;
    }

    private sealed class ConversionDefinitionRoot
    {
        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
        public List<ConversionStep> ConversionSteps { get; set; } = default!;
    }

}