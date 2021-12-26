using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using OpenCCSharp.Conversion;
using OpenCCSharp.Presets;
using OpenCCSharp.UnitTest.TestCases;
using Xunit;

namespace OpenCCSharp.UnitTest.Benchmarks
{
    public class ConverterBenchmarks
    {

        private volatile string? dummy;

        [Benchmark]
        [ArgumentsSource(nameof(EnumOpenCCTestArguments))]
        public void OpenCCTest(OpenCCTestArguments arguments)
        {
            foreach (var input in arguments.TestCases)
            {
                dummy = arguments.Converter.Convert(input);
            }
        }

        [IterationCleanup]
        public void Cleanup()
        {
            dummy = null;
        }

        public IEnumerable<OpenCCTestArguments> EnumOpenCCTestArguments()
        {
            var method = typeof(ConverterTests).GetMethod(nameof(ConverterTests.OpenCCTest));
            var argsAttributes = method!.GetCustomAttributes(typeof(InlineDataAttribute), true).Cast<InlineDataAttribute>();
            var cases = argsAttributes.SelectMany(a => a.GetData(method))
                .Select(a => new OpenCCTestArguments((ChineseConversionVariant)a[0], (ChineseConversionVariant)a[1], (string)a[2]))
                .ToList();
            return cases;
        }

        // private IEnumerable<OpenCCTestArguments>

        public class OpenCCTestArguments
        {
            public ChineseConversionVariant FromVariant { get; }

            public ChineseConversionVariant ToVariant { get; }

            public string CaseSetName { get; }

            public ScriptConverterBase Converter { get; }

            public List<string> TestCases { get; }

            public OpenCCTestArguments(ChineseConversionVariant fromVariant, ChineseConversionVariant toVariant, string caseSetName)
            {
                FromVariant = fromVariant;
                ToVariant = toVariant;
                CaseSetName = caseSetName;
                // FIXME sync wait on async values.
                Converter = ChineseConversionPresets.GetConverterAsync(fromVariant, toVariant).AsTask().GetAwaiter().GetResult();
                // We only keep input for sake of profiling.
                TestCases = OpenCCUtils.ReadTestCases(caseSetName).Select(t => t.Input).ToList();
            }

            /// <inheritdoc />
            public override string ToString() => $"{FromVariant} -> {ToVariant} [{CaseSetName}]";
        }

    }
}
