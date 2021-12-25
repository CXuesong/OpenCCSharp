using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Conversion
{
    public static class PlainTextConversionLookupTable
    {

        public static async Task LoadAsync(SortedStringPrefixDictionary dict, TextReader reader, PlainTextConversionLookupTableLoadOptions options = default)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            string? line;
            var reverseEntries
                = (options & PlainTextConversionLookupTableLoadOptions.ReverseEntries) == PlainTextConversionLookupTableLoadOptions.ReverseEntries;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var lineMemory = line.AsMemory().Trim();
                if (lineMemory.Length == 0 || lineMemory.Span.StartsWith("##")) continue;
                if (reverseEntries)
                {
                    var fields = lineMemory.ToString().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                    // Only keep the first converted phrase hit.
                    for (var i = 1; i < fields.Length; i++)
                        dict.TryAdd(fields[i], fields[0].AsMemory());
                }
                else
                {
                    var fields = lineMemory.ToString().Split((char[]?)null, 3, StringSplitOptions.RemoveEmptyEntries);
                    dict.Add(fields[0], fields[1].AsMemory());
                }
            }
        }

        public static async Task LoadAsync(SortedStringPrefixDictionary dict, Stream stream, PlainTextConversionLookupTableLoadOptions options = default)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            await LoadAsync(dict, reader, options);
        }

        public static async Task LoadAsync(SortedStringPrefixDictionary dict, string filePath, PlainTextConversionLookupTableLoadOptions options = default)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            await using var fs = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            await LoadAsync(dict, reader, options);
        }

    }

    [Flags]
    public enum PlainTextConversionLookupTableLoadOptions
    {
        None = 0,

        /// <summary>
        /// Reverts original/converted phrases pair when loading lookup table.
        /// </summary>
        ReverseEntries = 1,
    }
}
