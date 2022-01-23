using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using OpenCCSharp.Presets;

namespace OpenCCSharp.UnitTest.Benchmarks;

public class ConverterPresetBenchmarks
{

    [Benchmark]
    [Arguments(ChineseConversionVariant.Hans, ChineseConversionVariant.Hant)]
    [Arguments(ChineseConversionVariant.Hans, ChineseConversionVariant.Hani)]
    [Arguments(ChineseConversionVariant.Hans, ChineseConversionVariant.HK)]
    [Arguments(ChineseConversionVariant.Hans, ChineseConversionVariant.TW)]
    [Arguments(ChineseConversionVariant.Hant, ChineseConversionVariant.Hans)]
    [Arguments(ChineseConversionVariant.Hant, ChineseConversionVariant.Hani)]
    [Arguments(ChineseConversionVariant.HK, ChineseConversionVariant.Hans)]
    [Arguments(ChineseConversionVariant.TW, ChineseConversionVariant.Hans)]
    public async ValueTask PresetLoadTest(ChineseConversionVariant fromVariant, ChineseConversionVariant toVariant)
    {
        await ChineseConversionPresets.GetConverterAsync(fromVariant, toVariant);
    }

    [IterationCleanup]
    public void Cleanup()
    {
        ChineseConversionPresets.ClearConverterCache();
    }

}