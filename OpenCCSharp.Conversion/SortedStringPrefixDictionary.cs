using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace OpenCCSharp.Conversion;

public class SortedStringPrefixDictionary<TValue> : IReadOnlyStringPrefixDictionary<TValue>
{

    private readonly SortedList<ReadOnlyMemory<char>, TValue> _myDict = new(CharMemoryComparer.Default);
    private int maxKeyLength = 0;

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<ReadOnlyMemory<char>, TValue>> GetEnumerator() => _myDict.GetEnumerator();

    /// <inheritdoc />
    public int Count => _myDict.Count;

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlyMemory<char> key) => _myDict.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlyMemory<char> key, out TValue value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> Keys => _myDict.Keys;

    /// <inheritdoc />
    public IEnumerable<TValue> Values => _myDict.Values;

    /// <inheritdoc />
    public TValue this[ReadOnlySpan<char> key]
    {
        get
        {
            var index = BinarySearch(key);
            if (index >= 0) return _myDict.Values[index];
            throw new KeyNotFoundException();
        }
    }

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue value)
    {
        var index = BinarySearch(key);
        if (index >= 0)
        {
            value = _myDict.Values[index];
            return true;
        }
        value = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetLongestPrefixingKey(ReadOnlySpan<char> content, out ReadOnlyMemory<char> key)
    {
        for (var prefixLength = Math.Min(content.Length, this.maxKeyLength); prefixLength > 0; prefixLength--)
        {
            var index = BinarySearch(content[..prefixLength]);
            if (index >= 0)
            {
                key = _myDict.Keys[index];
                return true;
            }
        }
        key = default;
        return false;
    }

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content)
    {
        for (var prefixLength = 0; prefixLength < maxKeyLength; prefixLength++)
        {
            var index = BinarySearch(content[..prefixLength]);
            if (index >= 0)
            {
                yield return _myDict.Keys[index];
            }
        }
    }

    private int BinarySearch(ReadOnlySpan<char> key)
    {
        var keys = _myDict.Keys;
        int lo = 0, hi = keys.Count - 1;
        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            var diff = CharMemoryComparer.CompareBySpan(keys[m], key);
            if (diff < 0) lo = m + 1;
            else if (diff > 0) hi = m - 1;
            else return m;
        }
        return -1;
    }

    private class CharMemoryComparer : Comparer<ReadOnlyMemory<char>>
    {

        public new static CharMemoryComparer Default { get; } = new();

        /// <inheritdoc />
        public override int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
        {
            if (x.IsEmpty) return y.IsEmpty ? 0 : -1;
            if (y.IsEmpty) return 1;
            return x.Span.CompareTo(y.Span, StringComparison.Ordinal);
        }

        public static int CompareBySpan(ReadOnlyMemory<char> x, ReadOnlySpan<char> y)
        {
            if (x.IsEmpty) return y.IsEmpty ? 0 : -1;
            if (y.IsEmpty) return 1;
            return x.Span.CompareTo(y, StringComparison.Ordinal);
        }

    }

}