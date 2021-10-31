using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace OpenCCSharp.Conversion;

public class ChainedStringDictionaryLookup : IStringDictionaryLookup<ReadOnlyMemory<char>>
{

    private readonly List<IStringDictionaryLookup<ReadOnlyMemory<char>>> _myDicts;

    public ChainedStringDictionaryLookup(IEnumerable<IStringDictionaryLookup<ReadOnlyMemory<char>>> chainedDicts)
    {
        if (chainedDicts == null) throw new ArgumentNullException(nameof(chainedDicts));
        _myDicts = chainedDicts.ToList();
    }

    /// <inheritdoc />
    public ReadOnlyMemory<char> this[ReadOnlySpan<char> key]
        => TryGetValue(key, out var v) ? v : throw new KeyNotFoundException();

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlySpan<char> key)
    {
        foreach (var d in _myDicts)
            if (d.ContainsKey(key))
                return true;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlySpan<char> key, out ReadOnlyMemory<char> value)
    {
        var currentSpan = default(ReadOnlySpan<char>);
        value = default;
        foreach (var d in _myDicts)
        {
            if (currentSpan.IsEmpty)
            {
                // First match in the chain.
                if (d.TryGetValue(key, out value))
                {
                    currentSpan = value.Span;
                    // Match resulted in empty string. Stop here.
                    if (value.IsEmpty) return true;
                }
                continue;
            }
            // Attempt to match based on matched value in the previous dict in chain.
            if (d.TryGetValue(value.Span, out var next))
            {
                value = next;
                currentSpan = value.Span;
                // Match resulted in empty string. Stop here.
                if (value.IsEmpty) return true;
            }
        }
        return !currentSpan.IsEmpty;
    }

}
