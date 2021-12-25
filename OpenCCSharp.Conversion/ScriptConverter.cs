using System.Buffers;
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
            sourceConsumed += len;
            if (_conversionLookup.TryGetValue(segment, out var v))
            {
                if (destRest.Length < v.Length) return;
                v.Span.CopyTo(destRest);
                destRest = destRest[v.Length..];
                destinationConsumed += v.Length;
            }
            else
            {
                // Copy original segment content
                if (destRest.Length < len) return;
                segment.CopyTo(destRest);
                destRest = destRest[len..];
                destinationConsumed += len;
            }
        }
        Debug.Assert(srcRest.IsEmpty);
        completed = true;
    }
}
