using System.Collections.Concurrent;
using System.Numerics;
using System.Resources;
using System.Text.Json;
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

                // Merge small char arrays into large char pools to reduce 16B per object overhead.
                var charPool = GC.AllocateUninitializedArray<char>((int)BitOperations.RoundUpToPowerOf2(Math.Clamp((uint)(rs.Length / 8), 32, 512 * 1024)));
                var charPoolPos = 0;
                var charPoolLengthSum = charPool.Length;
                Memory<char> AllocateCharPoolMemory(int size)
                {
                    if (charPool.Length - charPoolPos < size)
                    {
                        // Note that StreamReader in EnumEntriesFromAsync has its own buffer...
                        var readerPosGuess = rs.Position >= rs.Length ? Math.Max(0, rs.Length - 256) : Math.Max(0, rs.Position - 512);
                        // Estimate how large the next pool should be.
                        charPool = GC.AllocateUninitializedArray<char>((int)BitOperations.RoundUpToPowerOf2(Math.Clamp(
                            (uint)((float)charPoolLengthSum / readerPosGuess * (rs.Length - readerPosGuess)),
                            Math.Max(64, (uint)size), 512 * 1024)));
                        charPoolLengthSum += charPool.Length;
                        charPoolPos = 0;
                    }
                    var m = charPool.AsMemory(charPoolPos, size);
                    charPoolPos += size;
                    return m;
                }
                await foreach (var (k, v) in PlainTextConversionLookupTable.EnumEntriesFromAsync(rs))
                {
                    if (reverse)
                    {
                        foreach (var v1 in v)
                        {
                            var m = AllocateCharPoolMemory(k.Length);
                            k.CopyTo(m);
                            dict.TryAdd(v1, m);
                        }
                    }
                    else
                    {
                        var m = AllocateCharPoolMemory(v[0].Length);
                        v[0].CopyTo(m);
                        dict.TryAdd(k, m);
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