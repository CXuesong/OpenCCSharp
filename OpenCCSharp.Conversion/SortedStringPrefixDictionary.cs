using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace OpenCCSharp.Conversion;

[DebuggerDisplay("Count = {Count}, MaxKeyLength = {_maxKeyLength}")]
public class SortedStringPrefixDictionary : IReadOnlyStringPrefixDictionary
{

    private readonly SortedList<ReadOnlyMemory<char>, ReadOnlyMemory<char>> _myDict;
    private int _maxKeyLength = 0;

    public SortedStringPrefixDictionary() : this(null)
    {
    }

    public SortedStringPrefixDictionary(IEnumerable<KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>>? dict)
    {
        if (dict is IDictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> d)
        {
            _myDict = new(d, CharMemoryComparer.Default);
            _maxKeyLength = _myDict.Count > 0 ? _myDict.Keys.Select(k => k.Length).Max() : 0;
        }
        else if (dict is not null)
        {
            _myDict = new(dict.TryGetNonEnumeratedCount(out var count) ? count : 0, CharMemoryComparer.Default);
            foreach (var (key, value) in dict) Add(key, value);
        }
        else
        {
            _myDict = new(CharMemoryComparer.Default);
        }
    }

    #region Mutation
    
    public void Add(string key, string value)
    {
        _myDict.Add(key.AsMemory(), value.AsMemory());
        _maxKeyLength = Math.Max(_maxKeyLength, key.Length);
    }

    public void Add(string key, ReadOnlyMemory<char> value)
    {
        _myDict.Add(key.AsMemory(), value);
        _maxKeyLength = Math.Max(_maxKeyLength, key.Length);
    }

    // n.b. It's caller's responsibility to keep key immutable.
    public void Add(ReadOnlyMemory<char> key, ReadOnlyMemory<char> value)
    {
        _myDict.Add(key, value);
        _maxKeyLength = Math.Max(_maxKeyLength, key.Length);
    }

    public bool TryAdd(string key, string value)
    {
        if (!_myDict.TryAdd(key.AsMemory(), value.AsMemory())) return false;
        _maxKeyLength = Math.Max(_maxKeyLength, key.Length);
        return true;
    }

    public bool TryAdd(string key, ReadOnlyMemory<char> value)
    {
        if (!_myDict.TryAdd(key.AsMemory(), value)) return false;
        _maxKeyLength = Math.Max(_maxKeyLength, key.Length);
        return true;
    }

    // n.b. It's caller's responsibility to keep key immutable.
    public bool TryAdd(ReadOnlyMemory<char> key, ReadOnlyMemory<char> value)
    {
        if (!_myDict.TryAdd(key, value)) return false;
        _maxKeyLength = Math.Max(_maxKeyLength, key.Length);
        return true;
    }

    #endregion

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>> GetEnumerator() => _myDict.GetEnumerator();

    /// <inheritdoc />
    public int Count => _myDict.Count;

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlyMemory<char> key) => _myDict.ContainsKey(key);

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlySpan<char> key) => BinarySearch(key) >= 0;

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlyMemory<char> key, out ReadOnlyMemory<char> value)
    {
        if (key.Length > _maxKeyLength)
        {
            value = default;
            return false;
        }
        return _myDict.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> Keys => _myDict.Keys;

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> Values => _myDict.Values;

    /// <inheritdoc />
    public ReadOnlyMemory<char> this[ReadOnlyMemory<char> key] => _myDict[key];

    /// <inheritdoc />
    public ReadOnlyMemory<char> this[ReadOnlySpan<char> key]
    {
        get
        {
            var index = BinarySearch(key);
            if (index >= 0) return _myDict.Values[index];
            throw new KeyNotFoundException();
        }
    }

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlySpan<char> key, out ReadOnlyMemory<char> value)
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
    public (int length, ReadOnlyMemory<char> value) TryGetLongestPrefixingKey(ReadOnlySpan<char> content)
    {
        for (var prefixLength = Math.Min(content.Length, this._maxKeyLength); prefixLength > 0; prefixLength--)
        {
            var index = BinarySearch(content[..prefixLength]);
            if (index >= 0)
            {
                return (_myDict.Keys[index].Length, _myDict.Values[index]);
            }
        }
        return (-1, default);
    }

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content)
    {
        var keys = new List<ReadOnlyMemory<char>>();
        for (var prefixLength = 0; prefixLength < _maxKeyLength; prefixLength++)
        {
            var index = BinarySearch(content[..prefixLength]);
            if (index >= 0)
            {
                keys.Add(_myDict.Keys[index]);
            }
        }
        return keys;
    }

    private int BinarySearch(ReadOnlySpan<char> key)
    {
        var keys = _myDict.Keys;
        int lo = 0, hi = keys.Count - 1;
        while (lo <= hi)
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