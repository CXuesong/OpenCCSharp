using System.Diagnostics;

namespace OpenCCSharp.Conversion;

[DebuggerDisplay("BaseMappings = {BaseMappings}")]
public class MergedStringPrefixMapping : IStringPrefixMapping
{

    private readonly List<IStringPrefixMapping> _myDicts;

    public MergedStringPrefixMapping(params IStringPrefixMapping[] baseMappings)
        : this((IEnumerable<IStringPrefixMapping>)baseMappings)
    {
    }

    public MergedStringPrefixMapping(IEnumerable<IStringPrefixMapping> baseMappings)
    {
        if (baseMappings == null) throw new ArgumentNullException(nameof(baseMappings));
        _myDicts = baseMappings.ToList();
        BaseMappings = _myDicts.AsReadOnly();
    }

    public IReadOnlyList<IStringPrefixMapping> BaseMappings { get; }

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
    public (int length, ReadOnlyMemory<char> value) TryGetLongestPrefixingKey(ReadOnlySpan<char> content)
    {
        var longestKeyLength = -1;
        var matchValue = ReadOnlyMemory<char>.Empty;
        foreach (var dict in _myDicts)
        {
            var (len, value) = dict.TryGetLongestPrefixingKey(content);
            if (len <= longestKeyLength) continue;
            longestKeyLength = len;
            matchValue = value;
        }
        return (longestKeyLength, matchValue);
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