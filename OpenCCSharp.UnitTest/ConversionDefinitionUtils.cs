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

namespace OpenCCSharp.UnitTest
{
    internal class ConversionDefinitionUtils
    {

        public static readonly string ConversionDefinitionsDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../ConversionDefinitions");


        public static async Task<ChainedScriptConverter> CreateConverterFromAsync(string configFileName)
        {
            ConversionDefinitionRoot config;
            await using (var fs = File.OpenRead(Path.Join(ConversionDefinitionsDir, configFileName)))
                config = JsonSerializer.Deserialize<ConversionDefinitionRoot>(fs, new JsonSerializerOptions
                         {
                             ReadCommentHandling = JsonCommentHandling.Skip,
                         })
                         ?? throw new InvalidOperationException("Config JSON resolves to null.");
            var converters = await config.ConversionSteps.ToAsyncEnumerable().SelectAwait(async step =>
            {
                var dicts = await step.Dictionaries
                    .ToAsyncEnumerable()
                    .SelectAwait(OpenCCUtils.GetDictionaryFromAsync)
                    .ToListAsync();
                var mergedMapping = new MergedStringPrefixMapping(dicts);
                var lexer = new LongestPrefixLexer(mergedMapping);
                return new ScriptConverter(lexer, mergedMapping);
            }).ToListAsync();
            return new ChainedScriptConverter(converters);
        }

        public class ConversionStep
        {
            public List<string> Dictionaries { get; set; }
        }

        public class ConversionDefinitionRoot
        {
            public string From { get; set; }
            public string To { get; set; }
            public List<ConversionStep> ConversionSteps { get; set; }
        }

    }
}
