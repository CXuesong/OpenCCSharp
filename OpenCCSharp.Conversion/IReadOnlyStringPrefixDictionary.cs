using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace OpenCCSharp.Conversion;

public interface IStringDictionaryLookup<TValue>
{

    TValue this[ReadOnlySpan<char> key] { get; }

    bool ContainsKey(ReadOnlySpan<char> key);

    bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue value);

}

public interface IReadOnlyStringDictionary<TValue> : IReadOnlyDictionary<ReadOnlyMemory<char>, TValue>, IStringDictionaryLookup<TValue>
{

    TValue IReadOnlyDictionary<ReadOnlyMemory<char>, TValue>.this[ReadOnlyMemory<char> key] => this[key.Span];

    bool IReadOnlyDictionary<ReadOnlyMemory<char>, TValue>.TryGetValue(ReadOnlyMemory<char> key, [MaybeNullWhen(false)] out TValue value)
        => this.TryGetValue(key.Span, out value);

    bool IReadOnlyDictionary<ReadOnlyMemory<char>, TValue>.ContainsKey(ReadOnlyMemory<char> key)
        => this.ContainsKey(key.Span);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

public interface IReadOnlyStringPrefixDictionary<TValue> : IReadOnlyStringDictionary<TValue>
{

    bool TryGetLongestPrefixingKey(ReadOnlySpan<char> content, out ReadOnlyMemory<char> key);

    IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content);

}
