using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Conversion;

public class MergedStringPrefixMapping : IStringPrefixMapping
{

    private readonly List<IStringPrefixMapping> _myDicts;

    /// <inheritdoc />
    public ReadOnlyMemory<char> this[ReadOnlySpan<char> key]
    {
        get
        {
            foreach (var dict in _myDicts)
                if (dict.TryGetValue(key, out var value))
                    return value;
            throw new KeyNotFoundException();
        }
    }

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlySpan<char> key)
    {
        foreach (var dict in _myDicts)
            if (dict.ContainsKey(key))
                return true;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlySpan<char> key, out ReadOnlyMemory<char> value)
    {
        foreach (var dict in _myDicts)
            if (dict.TryGetValue(key, out value))
                return true;
        value = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetLongestPrefixingKey(ReadOnlySpan<char> content, out ReadOnlyMemory<char> key)
    {
        key = default;
        var anyMatch = false;
        foreach (var dict in _myDicts)
        {
            if (dict.TryGetLongestPrefixingKey(content, out var key1) && (!anyMatch || key1.Length > key.Length))
            {
                anyMatch = true;
                key = key1;
            }
        }
        return anyMatch;
    }

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content)
    {
        var keys = new List<ReadOnlyMemory<char>>();
        foreach (var dict in _myDicts)
        {
            keys.AddRange(dict.EnumPrefixingKeys(content));
        }
        return keys;
    }

}