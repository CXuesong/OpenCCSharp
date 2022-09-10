namespace OpenCCSharp.Conversion;

public interface IScriptLexer
{

    int GetNextSegmentLength(ReadOnlySpan<char> content);

    //public IEnumerable<(int Start, int Length)> EnumSegments(ReadOnlySpan<char> content)
    //{
    //    if (content.IsEmpty) yield break;
    //    var start = 0;
    //    while (start < content.Length)
    //    {
    //        var rest = content[start..];
    //        var len = GetNextSegmentLength(rest);
    //        if (len <= 0) yield break;
    //        yield return (start, len);
    //        start += len;
    //    }
    //}

}
