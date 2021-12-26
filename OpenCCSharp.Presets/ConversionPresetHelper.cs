using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenCCSharp.Conversion;

namespace OpenCCSharp.Presets
{
    internal static class ConversionPresetHelper
    {

        private static readonly string OpenCCDictionaryDir = Path.Join(typeof(ChineseConversionPresets).Assembly.Location, "../vendor/OpenCC/dictionary");

        private static readonly string ConversionDefinitionsDir = Path.Join(typeof(ChineseConversionPresets).Assembly.Location, "../ConversionDefinitions");

        // <string, Task<SortedStringPrefixDictionary> | SortedStringPrefixDictionary>
        private static readonly ConcurrentDictionary<string, object> dictCache = new();

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

        public static async ValueTask<ChainedScriptConverter> CreateConverterFromAsync(string configFileName)
        {
            ConversionDefinitionRoot config;
            await using (var fs = File.OpenRead(Path.Join(ConversionDefinitionsDir, configFileName)))
                config = JsonSerializer.Deserialize<ConversionDefinitionRoot>(fs, new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                })
                         ?? throw new InvalidOperationException("Config JSON resolves to null.");
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

        private sealed class ConversionStep
        {
            public List<string> Dictionaries { get; set; }
        }

        private sealed class ConversionDefinitionRoot
        {
            public string From { get; set; }
            public string To { get; set; }
            public List<ConversionStep> ConversionSteps { get; set; }
        }

    }
}
