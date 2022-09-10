using System.Diagnostics;

namespace OpenCCSharp.Conversion;

/// <summary>
/// Converts one writing script to another one, based on the specified lexing and mapping rules.
/// </summary>
public class ScriptConverter : ScriptConverterBase
{

    private readonly IScriptLexer _lexer;
    private readonly IStringMapping _conversionLookup;

    public ScriptConverter(IScriptLexer lexer, IStringMapping conversionLookup)
    {
        _lexer = lexer;
        _conversionLookup = conversionLookup;
    }

    public override void Convert(ReadOnlySpan<char> source, Span<char> destination, out int sourceConsumed, out int destinationConsumed, out bool completed)
    {
        sourceConsumed = destinationConsumed = 0;
        if (source.IsEmpty)
        {
            completed = true;
            return;
        }
        var srcRest = source;
        var destRest = destination;
        completed = false;
        int len;
        while ((len = _lexer.GetNextSegmentLength(srcRest)) > 0)
        {
            var segment = srcRest[..len];
            srcRest = srcRest[len..];
            var converted = _conversionLookup.TryGetValue(segment, out var v) ? v.Span : segment;
            if (destRest.Length < converted.Length) return;
            converted.CopyTo(destRest);
            destRest = destRest[converted.Length..];
            sourceConsumed += len;
            destinationConsumed += converted.Length;
        }
        Debug.Assert(srcRest.IsEmpty);
        completed = true;
    }
}
