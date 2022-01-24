using System;
using System.Buffers;
using System.Diagnostics;

namespace OpenCCSharp.Conversion;

public abstract class ScriptConverterBase
{

    public abstract void Convert(ReadOnlySpan<char> source, Span<char> destination,
        out int sourceConsumed, out int destinationConsumed, out bool completed);

    public virtual string Convert(string source) => Convert(source.AsSpan());

    internal List<(char[] Buffer, int Length)> GetConvertedBuffers(ReadOnlySpan<char> source, int maxDestLength, out int sourceConsumed, out int destConsumed)
    {
        var buffers = new List<(char[] Buffer, int Length)>();
        if (maxDestLength == 0)
        {
            sourceConsumed = destConsumed = 0;
            return buffers;
        }
        var rest = source;
        sourceConsumed = 0;
        destConsumed = 0;
        var successful = false;
        const int MAX_ALLOWED_INFLATION_RATIO = 16;
        const int MAX_ALLOWED_INDETERMINISTIC_BUFFER_SIZE = 16*1024*1024;
        try
        {
            do
            {
                var maxBufferSize = maxDestLength > 0 ? (maxDestLength - destConsumed) : MAX_ALLOWED_INDETERMINISTIC_BUFFER_SIZE;
                var bufferSize = Math.Min(rest.Length, maxBufferSize);
                BUFFER_EXPAND_RETRY:
                var buffer = ArrayPool<char>.Shared.Rent(bufferSize);
                var bufferSpan = buffer.Length > maxBufferSize ? buffer.AsSpan(0, maxBufferSize) : buffer.AsSpan();
                Convert(rest, bufferSpan, out var sourceConsumedLocal, out var destConsumedLocal, out var completed);
                Debug.Assert(destConsumedLocal <= maxBufferSize);
                if (sourceConsumedLocal <= 0)
                {
                    Debug.Assert(!completed);
                    ArrayPool<char>.Shared.Return(buffer);
                    // The buffer is too small to write only one word.
                    if (bufferSize < maxBufferSize && bufferSpan.Length / rest.Length < MAX_ALLOWED_INFLATION_RATIO)
                    {
                        // Try to enlarge buffer during next iteration. Hopefully we'll make progress.
                        bufferSize = Math.Min(bufferSize * 2, maxBufferSize);
                        goto BUFFER_EXPAND_RETRY;
                    }
                    else
                    {
                        // We cannot help.
                        break;
                    }
                }
                buffers.Add((buffer, destConsumedLocal));
                // Consumed source / dest
                sourceConsumed += sourceConsumedLocal;
                destConsumed += destConsumedLocal;
                rest = rest[sourceConsumedLocal..];
            } while (!rest.IsEmpty);
            successful = true;
            return buffers;
        }
        finally
        {
            if (!successful)
            {
                foreach (var (buffer, _) in buffers) ArrayPool<char>.Shared.Return(buffer);
            }
        }
    }

    internal void ReleaseConvertedBuffers(List<(char[] Buffer, int Length)> buffers)
    {
        foreach (var (buffer, _) in buffers) ArrayPool<char>.Shared.Return(buffer);
    }

    public virtual string Convert(ReadOnlySpan<char> source)
    {
        if (source.IsEmpty) return "";
        var buffers = GetConvertedBuffers(source, -1, out _, out var destLength);
        try
        {
            if (destLength == 0) return "";
            if (buffers.Count == 1) return new string(buffers[0].Buffer, 0, buffers[0].Length);
            return string.Create(destLength, buffers, (sp, bufs) =>
            {
                foreach (var (buf, len) in bufs)
                {
                    buf.AsSpan(0, len).CopyTo(sp);
                    sp = sp[len..];
                }
            });
        }
        finally
        {
            ReleaseConvertedBuffers(buffers);
        }
    }
}