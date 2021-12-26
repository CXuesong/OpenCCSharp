using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Conversion;

/// <summary>
/// A script converter implementation composed of multiple ScriptConverter instances executed one by one.
/// </summary>
public class ChainedScriptConverter : ScriptConverterBase
{

    private readonly List<ScriptConverterBase> underlyingConverters;

    public ChainedScriptConverter(IEnumerable<ScriptConverterBase> underlyingConverters)
    {
        if (underlyingConverters == null) throw new ArgumentNullException(nameof(underlyingConverters));
        this.underlyingConverters = underlyingConverters.ToList();
    }

    /// <inheritdoc />
    /// <remarks>
    /// The current implementation will not consume <paramref name="source"/> at all
    /// if <paramref name="destination"/> is not large enough to contain the whole string.
    /// </remarks>
    public override void Convert(ReadOnlySpan<char> source, Span<char> destination, out int sourceConsumed, out int destinationConsumed, out bool completed)
    {
        if (source.Overlaps(destination)) throw new ArgumentException("Source and destination span should not overlap with each other.");
        if (source.IsEmpty)
        {
            sourceConsumed = destinationConsumed = 0;
            completed = true;
            return;
        }

        if (underlyingConverters.Count == 0)
        {
            if (source.Length <= destination.Length)
            {
                sourceConsumed = destinationConsumed = source.Length;
                completed = true;
                source.CopyTo(destination);
            }
            else
            {
                sourceConsumed = destinationConsumed = destination.Length;
                completed = false;
                source[..destination.Length].CopyTo(destination);
            }
            return;
        }
            
        if (underlyingConverters.Count == 1)
        {
            underlyingConverters[0].Convert(source, destination, out sourceConsumed, out destinationConsumed, out completed);
            return;
        }

        char[]? buffer = null;
        var sourceLocal = Span<char>.Empty; // This indicates "source"
        var destLocal = destination;
        completed = true;
        sourceConsumed = destinationConsumed = 0;
        try
        {
            foreach (var converter in underlyingConverters)
            {
                if (destLocal.IsEmpty)
                {
                    // Allocate temp buffer.
                    buffer = ArrayPool<char>.Shared.Rent(destination.Length);
                    destLocal = buffer.AsSpan(0, destination.Length);
                }
                converter.Convert(sourceLocal.IsEmpty ? source : sourceLocal, destLocal,
                    out var sourceConsumedLocal, out var destConsumedLocal,
                    out var completedLocal);
                if (!completedLocal)
                {
                    // As for now, we do not have any mechanism to properly back off the last "word"
                    // in order to fit in the destination buffer.
                    // FIXME Just do nothing and tell the caller buffer is not large enough.
                    sourceConsumed = destinationConsumed = 0;
                    completed = false;
                    return;
                }
                if (sourceConsumed == 0) sourceConsumed = sourceConsumedLocal;
                destinationConsumed = destConsumedLocal;
                // Swap buffers.
                var tempSpan = destLocal;
                destLocal = sourceLocal;
                sourceLocal = tempSpan;
            }
            // Now sourceLocal contains converted string
            // It can either be `destination` or `buffer`.
            if (sourceLocal != destination)
            {
                // sourceLocal is `buffer`. Copy it to destination.
                Debug.Assert(sourceLocal == buffer.AsSpan(destination.Length));
                sourceLocal.CopyTo(destination);
            }
        }
        finally
        {
            if (buffer != null) ArrayPool<char>.Shared.Return(buffer);
        }
    }

    /// <inheritdoc />
    public override string Convert(ReadOnlySpan<char> source)
    {
        if (source.IsEmpty) return "";
        if (underlyingConverters.Count == 0) return source.ToString();
        if (underlyingConverters.Count == 1) return underlyingConverters[0].Convert(source);
        char[]? buffer = null;
        var sourceLocal = source;
        try
        {
            foreach (var converter in underlyingConverters)
            {
                var converted = converter.GetConvertedBuffers(sourceLocal, -1, out var sourceConsumed, out var destConsumed);
                try
                {
                    Debug.Assert(sourceConsumed == sourceLocal.Length);
                    if (buffer == null || buffer.Length < destConsumed)
                    {
                        if (buffer != null) ArrayPool<char>.Shared.Return(buffer);
                        buffer = ArrayPool<char>.Shared.Rent(destConsumed);
                    }
                    var pos = 0;
                    foreach (var (b, len) in converted)
                    {
                        Array.Copy(b, 0, buffer, pos, len);
                        pos += len;
                    }
                    sourceLocal = buffer.AsSpan(0, destConsumed);
                }
                finally
                {
                    converter.ReleaseConvertedBuffers(converted);
                }
            }
            return new string(buffer!, 0, sourceLocal.Length);
        }
        finally
        {
            if (buffer != null) ArrayPool<char>.Shared.Return(buffer);
        }
    }

    /// <inheritdoc />
    public override string Convert(string source)
    {
        if (source.Length == 0) return "";
        if (underlyingConverters.Count == 0) return source;
        if (underlyingConverters.Count == 1) return underlyingConverters[0].Convert(source);
        return Convert(source.AsSpan());
    }

}