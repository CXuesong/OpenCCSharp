using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Conversion
{
    public static class PlainTextConversionLookupTable
    {

        public static async Task Load(SortedStringPrefixDictionary<ReadOnlyMemory<char>> dict, TextReader reader)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                line = line.Trim();
                if (line.Length == 0 || line.StartsWith("##")) continue;
                var fields = line.Split((char[]?)null, 3, StringSplitOptions.RemoveEmptyEntries);
                dict.Add(fields[0], fields[1].AsMemory());
            }
        }

        public static async Task Load(SortedStringPrefixDictionary<ReadOnlyMemory<char>> dict, Stream stream)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            await Load(dict, reader);
        }

        public static async Task Load(SortedStringPrefixDictionary<ReadOnlyMemory<char>> dict, string filePath)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            await using var fs = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            await Load(dict, reader);
        }

    }
}
