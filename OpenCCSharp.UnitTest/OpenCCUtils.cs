using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using O9d.Json.Formatting;
using OpenCCSharp.Conversion;

namespace OpenCCSharp.UnitTest;

internal static class OpenCCUtils
{

    public static readonly string OpenCCConversionConfigDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/config");

    public static readonly string OpenCCDictionaryDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/dictionary");

    public static readonly string OpenCCTestCasesDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/testcases");

    // <string, Task<SortedStringPrefixDictionary> | SortedStringPrefixDictionary>
    private static readonly ConcurrentDictionary<string, object> dictCache = new();

    public static IEnumerable<(string Input, string Output)> ReadTestCases(string caseSetName)
    {
        return File.ReadLines(Path.Join(OpenCCTestCasesDir, caseSetName + ".in"))
            .Zip(File.ReadLines(Path.Join(OpenCCTestCasesDir, caseSetName + ".ans")));
    }

}
