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
        var (len, _) = _Mapping.TryGetLongestPrefixingKey(content);
        if (len > 0)
            return len;
        // Skip to next Unicode point (rune)
        if (Rune.DecodeFromUtf16(content, out _, out len) == OperationStatus.Done)
            return len;
        return 1;
    }
}
