using System.Collections.Concurrent;
using System.Resources;
using System.Text.Json;
using OpenCCSharp.Conversion;
using TriesSharp.Collections;

namespace OpenCCSharp.Presets;

internal static class ConversionPresetHelper
{

    private static readonly string OpenCCDictionariesNamespacePrefix = typeof(ConversionPresetHelper).Namespace + ".ConversionDictionaries.";

    private static readonly string ConversionDefinitionsNamespacePrefix = typeof(ConversionPresetHelper).Namespace + ".ConversionDefinitions.";

    // <string, Task<TrieStringPrefixDictionary> | WeakReference<TrieStringPrefixDictionary>>
    private static readonly ConcurrentDictionary<string, object> dictCache = new();

    public static ValueTask<TrieStringPrefixDictionary> GetDictionaryFromAsync(string dictFileName)
    {
        static async Task<TrieStringPrefixDictionary> ValueFactory(string fn, ConcurrentDictionary<string, object> dc)
        {
            await Task.Yield();
            Trie<ReadOnlyMemory<char>> trie;
            var resourceName = OpenCCDictionariesNamespacePrefix + fn;
            await using (var rs = typeof(ConversionPresetHelper).Assembly.GetManifestResourceStream(resourceName))
            {
                if (rs == null)
                    throw new MissingManifestResourceException($"Failed to retrieve the manifest resource: {resourceName}.");

                trie = await TrieSerializer.Deserialize(rs);
            }

            var dict = new TrieStringPrefixDictionary(trie);
            dc[fn] = new WeakReference(dict);
            return dict;
        }

        RETRY:
        var cached = dictCache.GetOrAdd(dictFileName, static (fn, dc) => ValueFactory(fn, dc), dictCache);
        if (cached is WeakReference wr)
        {
            var dict = (TrieStringPrefixDictionary?)wr.Target;
            if (dict == null)
            {
                dictCache.TryRemove(new(dictFileName, wr));
                goto RETRY;
            }
            return new(dict);
        }
        if (cached is Task<TrieStringPrefixDictionary> t)
        {
            return new(t);
        }

        throw new InvalidOperationException();
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