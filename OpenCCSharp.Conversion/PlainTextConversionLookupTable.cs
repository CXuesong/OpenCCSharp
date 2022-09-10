using System.Buffers;
using System.Text;

namespace OpenCCSharp.Conversion;

public static class PlainTextConversionLookupTable
{

    public static async IAsyncEnumerable<KeyValuePair<ReadOnlyMemory<char>, IReadOnlyList<ReadOnlyMemory<char>>>> EnumEntriesFromAsync(
        TextReader reader,
        PlainTextConversionLookupTableLoadOptions options = default)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var keyWriter = new ArrayBufferWriter<char>(16);
        var valueWriter = new ArrayBufferWriter<char>(16);
        var lastValueStartIndex = 0;
        var valueList = new List<ReadOnlyMemory<char>>(4);
        using var bufferOwner = MemoryPool<char>.Shared.Rent(1024);
        var readerBuffer = bufferOwner.Memory;
        // begin of line
        const int STATE_BOL = 0;
        const int STATE_BOL_SHARP = 1;
        const int STATE_KEY = 2;
        const int STATE_VALUE_START = 3;
        const int STATE_VALUE = 4;
        const int STATE_COMMENT = 5;
        var state = STATE_BOL;
        int readCount;
        while ((readCount = await reader.ReadAsync(readerBuffer)) > 0)
        {
            for (var i = 0; i < readCount; i++)
            {
                var c = readerBuffer.Span[i];
                switch (state)
                {
                    case STATE_BOL:
                        if (char.IsWhiteSpace(c)) break;
                        if (c == '#')
                        {
                            state = STATE_BOL_SHARP;
                            break;
                        }
                        keyWriter.GetSpan(1)[0] = c;
                        keyWriter.Advance(1);
                        state = STATE_KEY;
                        break;
                    case STATE_BOL_SHARP:
                        if (c == '#')
                        {
                            state = STATE_COMMENT;
                            break;
                        }
                        keyWriter.GetSpan(1)[0] = '#';
                        keyWriter.GetSpan(2)[1] = c;
                        keyWriter.Advance(2);
                        break;
                    case STATE_KEY:
                        if (char.IsWhiteSpace(c))
                        {
                            // commit key
                            state = STATE_VALUE_START;
                            goto case STATE_VALUE_START;
                        }
                        keyWriter.GetSpan(1)[0] = c;
                        keyWriter.Advance(1);
                        break;
                    case STATE_VALUE_START:
                        if (c is '\r' or '\n')
                        {
                            // commit pair
                            if (valueList.Count == 0)
                                throw new FormatException("Expect conversion table to contain at least 2 fields per line.");
                            yield return KeyValuePair.Create(keyWriter.WrittenMemory, (IReadOnlyList<ReadOnlyMemory<char>>)valueList);
                            keyWriter.Clear();
                            valueList.Clear();
                            valueWriter.Clear();
                            lastValueStartIndex = 0;
                            state = STATE_BOL;
                            break;
                        }
                        if (char.IsWhiteSpace(c)) break;
                        valueWriter.GetSpan(1)[0] = c;
                        valueWriter.Advance(1);
                        state = STATE_VALUE;
                        break;
                    case STATE_VALUE:
                        if (char.IsWhiteSpace(c))
                        {
                            // commit value
                            valueList.Add(valueWriter.WrittenMemory[lastValueStartIndex..]);
                            lastValueStartIndex = valueWriter.WrittenCount;
                            state = STATE_VALUE_START;
                            goto case STATE_VALUE_START;
                        }
                        valueWriter.GetSpan(1)[0] = c;
                        valueWriter.Advance(1);
                        break;
                    case STATE_COMMENT:
                        if (c is '\r' or '\n') state = STATE_BOL;
                        break;
                }
            }
        }
        if (state is STATE_KEY or STATE_VALUE_START or STATE_VALUE)
        {
            if (state == STATE_VALUE)
                valueList.Add(valueWriter.WrittenMemory.ToArray());
            if (valueList.Count == 0)
                throw new FormatException("Expect conversion table to contain at least 2 fields per line.");
            yield return KeyValuePair.Create(keyWriter.WrittenMemory, (IReadOnlyList<ReadOnlyMemory<char>>)valueList);
        }
    }

    public static async IAsyncEnumerable<KeyValuePair<ReadOnlyMemory<char>, IReadOnlyList<ReadOnlyMemory<char>>>> EnumEntriesFromAsync(
        Stream stream,
        PlainTextConversionLookupTableLoadOptions options = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        await foreach (var p in EnumEntriesFromAsync(reader, options)) yield return p;
    }

    public static async IAsyncEnumerable<KeyValuePair<ReadOnlyMemory<char>, IReadOnlyList<ReadOnlyMemory<char>>>> EnumEntriesFromAsync(
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