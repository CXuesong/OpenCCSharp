﻿using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using OpenCCSharp.Conversion;
using OpenCCSharp.Presets;

namespace OpenCCSharp.UnitTest.Benchmarks;

public class ConverterBulkBenchmarks
{

    [Benchmark]
    [ArgumentsSource(nameof(GetBulkConversionTestArguments))]
    public void BulkConversionTest(BulkConversionTestArguments arguments)
    {
        arguments.Converter.Convert(arguments.Text);
    }

    public static IEnumerable<BulkConversionTestArguments> GetBulkConversionTestArguments()
    {
        var s2t = ChineseConversionPresets.GetConverterAsync(ChineseConversionVariant.Hans, ChineseConversionVariant.Hant)
            .AsTask().GetAwaiter().GetResult();
        var t2s = ChineseConversionPresets.GetConverterAsync(ChineseConversionVariant.Hant, ChineseConversionVariant.Hans)
            .AsTask().GetAwaiter().GetResult();

        static string BuildRepeatedSequence(int iterations)
        {
            // https://github.com/BYVoid/OpenCC/blob/556ed22496d650bd0b13b6c163be9814637970ae/src/benchmark/Performance.cpp#L101
            // Length = 27
            const string stem = "Open Chinese Convert 開放中文轉換";
            var sb = new StringBuilder(stem.Length * iterations);
            for (var i = 0; i < iterations; i++) sb.Append(stem);
            return sb.ToString();
        }

        yield return new("Zuozhuan Hans -> Hant", OpenCCUtils.LoadBenchmarkText(), s2t);
        yield return new("DupSequence 100 Hant -> Hans", BuildRepeatedSequence(100), t2s);
        yield return new("DupSequence 1000 Hant -> Hans", BuildRepeatedSequence(1000), t2s);
        yield return new("DupSequence 10000 Hant -> Hans", BuildRepeatedSequence(10000), t2s);
        yield return new("DupSequence 100000 Hant -> Hans", BuildRepeatedSequence(100000), t2s);
    }

    public record BulkConversionTestArguments(string Name, string Text, ScriptConverterBase Converter)
    {
        /// <inheritdoc />
        public override string ToString() => $"{Name} ({Text.Length} chars)";
    }

}