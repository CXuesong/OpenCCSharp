using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace OpenCCSharp.Conversion;

public interface IStringMapping
{

    ReadOnlyMemory<char> this[ReadOnlySpan<char> key] { get; }

    bool ContainsKey(ReadOnlySpan<char> key);

    bool TryGetValue(ReadOnlySpan<char> key, out ReadOnlyMemory<char> value);

}

public interface IStringPrefixMapping : IStringMapping
{

    bool TryGetLongestPrefixingKey(ReadOnlySpan<char> content, out ReadOnlyMemory<char> key);

    IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content);

}

public interface IReadOnlyStringPrefixDictionary : IReadOnlyDictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>, IStringMapping, IStringPrefixMapping
{

    ReadOnlyMemory<char> IReadOnlyDictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>.this[ReadOnlyMemory<char> key] => this[key.Span];

    bool IReadOnlyDictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>.TryGetValue(ReadOnlyMemory<char> key, out ReadOnlyMemory<char> value)
        => this.TryGetValue(key.Span, out value);

    bool IReadOnlyDictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>.ContainsKey(ReadOnlyMemory<char> key)
        => this.ContainsKey(key.Span);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
