using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Conversion;

public static class PlainTextConversionLookupTable
{

    public static async IAsyncEnumerable<KeyValuePair<ReadOnlyMemory<char>, IList<ReadOnlyMemory<char>>>> EnumEntriesFromAsync(
        TextReader reader,
        PlainTextConversionLookupTableLoadOptions options = default)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        string? line;
        var valueList = new List<ReadOnlyMemory<char>>();
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var lineMemory = line.AsMemory().Trim();
            if (lineMemory.Length == 0 || lineMemory.Span.StartsWith("##")) continue;
            var fields = lineMemory.ToString().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length < 2) throw new FormatException("Expect conversion table to contain at least 2 fields per line.");
            valueList.Clear();
            valueList.AddRange(fields.Skip(1).Select(f => f.AsMemory()));
            yield return KeyValuePair.Create(fields[0].AsMemory(), (IList<ReadOnlyMemory<char>>)valueList);
        }
    }

    public static async IAsyncEnumerable<KeyValuePair<ReadOnlyMemory<char>, IList<ReadOnlyMemory<char>>>> EnumEntriesFromAsync(
        Stream stream,
        PlainTextConversionLookupTableLoadOptions options = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        await foreach (var p in EnumEntriesFromAsync(reader, options)) yield return p;
    }

    public static async IAsyncEnumerable<KeyValuePair<ReadOnlyMemory<char>, IList<ReadOnlyMemory<char>>>> EnumEntriesFromAsync(
        string filePath,
        PlainTextConversionLookupTableLoadOptions options = default)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));
        await using var fs = new FileStream(filePath,
            FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        await foreach (var p in EnumEntriesFromAsync(reader, options)) yield return p;
    }

}

[Flags]
public enum PlainTextConversionLookupTableLoadOptions
{
    None = 0,
}