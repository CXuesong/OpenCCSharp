using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using OpenCCSharp.Presets;

namespace OpenCCSharp.UnitTest.Benchmarks;

public class ConverterPresetBenchmarks
{

    [Benchmark]
    [Arguments(ChineseConversionVariant.Hans, ChineseConversionVariant.Hant)]
    [Arguments(ChineseConversionVariant.Hans, ChineseConversionVariant.HK)]
    [Arguments(ChineseConversionVariant.Hans, ChineseConversionVariant.TW)]
    [Arguments(ChineseConversionVariant.Hant, ChineseConversionVariant.Hans)]
    [Arguments(ChineseConversionVariant.Hant, ChineseConversionVariant.HK)]
    [Arguments(ChineseConversionVariant.Hant, ChineseConversionVariant.TW)]
    [Arguments(ChineseConversionVariant.HK, ChineseConversionVariant.Hans)]
    [Arguments(ChineseConversionVariant.HK, ChineseConversionVariant.Hant)]
    [Arguments(ChineseConversionVariant.TW, ChineseConversionVariant.Hans)]
    [Arguments(ChineseConversionVariant.TW, ChineseConversionVariant.Hant)]
    [Arguments(ChineseConversionVariant.Kyujitai, ChineseConversionVariant.Shinjiatai)]
    [Arguments(ChineseConversionVariant.Shinjiatai, ChineseConversionVariant.Kyujitai)]
    public async ValueTask PresetLoadTest(ChineseConversionVariant fromVariant, ChineseConversionVariant toVariant)
    {
        await ChineseConversionPresets.GetConverterAsync(fromVariant, toVariant);
        // Unfortunately, just using the [IterationSetup] attribute is not enough to get stable results.
        // You also need to make sure that the benchmark itself performs enough of computations for a single invocation to run longer than 100ms.
        // If you don't, your benchmark will be entirely invalid.
        ChineseConversionPresets.ClearConverterCache();
    }

}