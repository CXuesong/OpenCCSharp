using System.Buffers;
using System.Diagnostics;

namespace OpenCCSharp.Conversion;

public class ScriptConverter
{

    private readonly IScriptLexer _lexer;
    private readonly IStringMapping _conversionLookup;

    public ScriptConverter(IScriptLexer lexer, IStringMapping conversionLookup)
    {
        _lexer = lexer;
        _conversionLookup = conversionLookup;
    }

    public void Convert(ReadOnlySpan<char> source, Span<char> destination, out int sourceConsumed, out int destinationConsumed, out bool completed)
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

    public string Convert(string source) => Convert(source.AsSpan());

    public string Convert(ReadOnlySpan<char> source)
    {
        if (source.IsEmpty) return "";
        var rest = source;
        var buffers = new List<(char[] Buffer, int Length)>();
        var destLength = 0;
        do
        {
            // Assumes the converted content has the same length of input.
            var buffer = ArrayPool<char>.Shared.Rent(rest.Length);
            Convert(rest, buffer.AsSpan(), out var sourceConsumed, out var destConsumed, out var completed);
            Debug.Assert(sourceConsumed > 0);
            buffers.Add((buffer, destConsumed));
            destLength += destConsumed;
            rest = source[sourceConsumed..];
            Debug.Assert(!(completed && !rest.IsEmpty));
        } while (!rest.IsEmpty);
        if (destLength == 0) return "";
        return string.Create(destLength, buffers, (sp, bufs) =>
        {
            foreach (var (buf, len) in bufs)
            {
                buf.AsSpan(0, len).CopyTo(sp);
                sp = sp[len..];
            }
        });
    }

}
