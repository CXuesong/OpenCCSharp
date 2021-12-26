using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

    public static readonly string OpenCCTestCasesDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/testcases");

    public static readonly string OpenCCBenchmarkDir = Path.Join(typeof(OpenCCUtils).Assembly.Location, "../vendor/OpenCC/benchmark");

    public static IEnumerable<(string Input, string Output)> ReadTestCases(string caseSetName)
    {
        return File.ReadLines(Path.Join(OpenCCTestCasesDir, caseSetName + ".in"))
            .Zip(File.ReadLines(Path.Join(OpenCCTestCasesDir, caseSetName + ".ans")));
    }

    public static string LoadBenchmarkText()
    {
        var text = File.ReadAllText(Path.Join(OpenCCBenchmarkDir, "zuozhuan.txt"));
        // Sanity check
        Trace.Assert(text.StartsWith("左传"));
        Trace.Assert(text.Length > 615 * 1024);
        return text;
    }

}
