using System;
using System.Buffers;
using System.Text;

namespace OpenCCSharp.Conversion;

public class LongestPrefixLexer : IScriptLexer
{
    private readonly IStringPrefixMapping _Mapping;

    public LongestPrefixLexer(IStringPrefixMapping mapping)
    {
        _Mapping = mapping;
    }

    /// <inheritdoc />
    public int GetNextSegmentLength(ReadOnlySpan<char> content)
    {
        if (content.IsEmpty) return 0;
        if (_Mapping.TryGetLongestPrefixingKey(content, out var key))
            return key.Length;
        // Skip to next Unicode point (rune)
        if (Rune.DecodeFromUtf16(content, out _, out var len) == OperationStatus.Done)
            return len;
        return 1;
    }
}
