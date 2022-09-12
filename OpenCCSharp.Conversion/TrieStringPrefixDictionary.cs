using TriesSharp.Collections;

namespace OpenCCSharp.Conversion;

public class TrieStringPrefixDictionary : IReadOnlyStringPrefixDictionary
{

    public TrieStringPrefixDictionary() : this(new())
    {
    }

    public TrieStringPrefixDictionary(Trie<ReadOnlyMemory<char>> trie)
    {
        Trie = trie ?? throw new ArgumentNullException(nameof(trie));
    }

    /// <summary>Gets the underlying trie.</summary>
    public Trie<ReadOnlyMemory<char>> Trie { get; }

    #region Mutation

    public void Add(string key, string value)
    {
        Trie.Add(key, value.AsMemory());
    }

    public void Add(string key, ReadOnlyMemory<char> value)
    {
        Trie.Add(key, value);
    }

    // n.b. It's caller's responsibility to keep key immutable.
    public void Add(ReadOnlyMemory<char> key, ReadOnlyMemory<char> value)
    {
        Trie.Add(key, value);
    }

    public bool TryAdd(string key, string value)
    {
        if (!Trie.ContainsKey(key))
        {
            Trie.Add(key, value.AsMemory());
            return true;
        }
        return false;
    }

    public bool TryAdd(string key, ReadOnlyMemory<char> value)
    {
        if (!Trie.ContainsKey(key))
        {
            Trie.Add(key, value);
            return true;
        }
        return false;
    }

    // n.b. It's caller's responsibility to keep key immutable.
    public bool TryAdd(ReadOnlyMemory<char> key, ReadOnlyMemory<char> value)
    {
        if (!Trie.ContainsKey(key))
        {
            Trie.Add(key, value);
            return true;
        }
        return false;
    }

    public void TrimExcess() => Trie.TrimExcess();

    #endregion

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>> GetEnumerator() => Trie.GetEnumerator();

    /// <inheritdoc />
    public int Count => Trie.Count;

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> Keys => Trie.Keys;

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> Values => Trie.Values;

    /// <inheritdoc />
    public ReadOnlyMemory<char> this[ReadOnlySpan<char> key] => Trie[key];

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlySpan<char> key) => Trie.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlySpan<char> key, out ReadOnlyMemory<char> value) => Trie.TryGetValue(key, out value);

    /// <inheritdoc />
    public (int length, ReadOnlyMemory<char> value) TryGetLongestPrefixingKey(ReadOnlySpan<char> content)
        => Trie.MatchLongestPrefix(content);

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content)
        => Trie.EnumEntriesFromPrefix(content).Select(p => p.Key);
}