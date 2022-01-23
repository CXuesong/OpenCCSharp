﻿using TriesSharp.Collections;

namespace OpenCCSharp.Conversion;

public class TrieStringPrefixDictionary : IReadOnlyStringPrefixDictionary
{
    private readonly Trie<ReadOnlyMemory<char>> trie = new();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>> GetEnumerator() => trie.GetEnumerator();

    /// <inheritdoc />
    public int Count => trie.Count;

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> Keys => trie.Keys;

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> Values => trie.Values;

    /// <inheritdoc />
    public ReadOnlyMemory<char> this[ReadOnlySpan<char> key] => trie[key];

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlySpan<char> key) => trie.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlySpan<char> key, out ReadOnlyMemory<char> value) => trie.TryGetValue(key, out value);

    /// <inheritdoc />
    public (int length, ReadOnlyMemory<char> value) TryGetLongestPrefixingKey(ReadOnlySpan<char> content)
        => trie.MatchLongestPrefix(content);

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content)
        => trie.EnumEntriesFromPrefix(content).Select(p => p.Key);
}