using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Conversion;

public interface IReadOnlyStringPrefixDictionary<TValue> : IReadOnlyDictionary<ReadOnlyMemory<char>, TValue>
{

    TValue this[ReadOnlySpan<char> key] { get; }

    TValue IReadOnlyDictionary<ReadOnlyMemory<char>, TValue>.this[ReadOnlyMemory<char> key] => this[key.Span];

    bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue value);

    bool TryGetLongestPrefixingKey(ReadOnlySpan<char> content, out ReadOnlyMemory<char> key);

    IEnumerable<ReadOnlyMemory<char>> EnumPrefixingKeys(ReadOnlySpan<char> content);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

