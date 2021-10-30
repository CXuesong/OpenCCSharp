namespace OpenCCSharp.Conversion;

public class ScriptConverter
{
    private readonly IScriptLexer _lexer;
    private readonly IReadOnlyStringPrefixDictionary<ReadOnlyMemory<char>> _conversionDict;

    public ScriptConverter(IScriptLexer lexer, IReadOnlyStringPrefixDictionary<ReadOnlyMemory<char>> conversionDict)
    {
        _lexer = lexer;
        _conversionDict = conversionDict;
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
            srcRest = source[len..];
            sourceConsumed += len;
            if (_conversionDict.TryGetValue(segment, out var v))
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
                srcRest[..len].CopyTo(destRest);
                destRest = destRest[len..];
                destinationConsumed += len;
            }
        }
        completed = true;
    }

}
