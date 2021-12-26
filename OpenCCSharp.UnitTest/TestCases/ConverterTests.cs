using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using OpenCCSharp.Conversion;
using OpenCCSharp.Presets;
using OpenCCSharp.UnitTest.Benchmarks;
using Xunit;
using Xunit.Abstractions;

namespace OpenCCSharp.UnitTest.TestCases;

public class ConverterTests : UnitTestsBase
{

    /// <inheritdoc />
    public ConverterTests(ITestOutputHelper output) : base(output)
    {
    }

    private void AssertConversionPair(ScriptConverterBase forwardConverter, ScriptConverterBase? backwardConverter, string text1, string text2)
    {
        Assert.Equal(text2, forwardConverter.Convert(text1));
        if (backwardConverter != null)
            Assert.Equal(text1, backwardConverter.Convert(text2));
    }

    [Fact]
    public async Task Test1()
    {
        var s2t = await ChineseConversionPresets.GetConverterAsync(ChineseConversionVariant.Hans, ChineseConversionVariant.Hant);
        var t2s = await ChineseConversionPresets.GetConverterAsync(ChineseConversionVariant.Hant, ChineseConversionVariant.Hans);
        var s2twp = await ChineseConversionPresets.GetConverterAsync(ChineseConversionVariant.Hans, ChineseConversionVariant.TW);
        var tw2sp = await ChineseConversionPresets.GetConverterAsync(ChineseConversionVariant.TW, ChineseConversionVariant.Hans);
        AssertConversionPair(s2t, t2s, "调试", "調試");
        AssertConversionPair(s2twp, tw2sp, "调试", "除錯");
        AssertConversionPair(s2t, t2s, "查看中文繁简转换单元测试结果，以调试可能出现的代码逻辑错误。", "查看中文繁簡轉換單元測試結果，以調試可能出現的代碼邏輯錯誤。");
        AssertConversionPair(s2twp, tw2sp, "查看中文繁简转换单元测试结果，以调试可能出现的代码逻辑错误。", "檢視中文繁簡轉換單元測試結果，以除錯可能出現的程式碼邏輯錯誤。");
    }

    /// <summary>
    /// Test cases ported from OpenCC C++ project.
    /// </summary>
    [Theory]
    [InlineData(ChineseConversionVariant.Hans, ChineseConversionVariant.Hant, "s2t")]
    [InlineData(ChineseConversionVariant.Hans, ChineseConversionVariant.HK, "s2hk")]
    [InlineData(ChineseConversionVariant.Hans, ChineseConversionVariant.TW, "s2twp")]
    [InlineData(ChineseConversionVariant.Hant, ChineseConversionVariant.Hani, "t2jp")]
    [InlineData(ChineseConversionVariant.Hant, ChineseConversionVariant.Hans, "t2s")]
    [InlineData(ChineseConversionVariant.HK, ChineseConversionVariant.Hans, "hk2s")]
    [InlineData(ChineseConversionVariant.TW, ChineseConversionVariant.Hans, "tw2sp")]
    [InlineData(ChineseConversionVariant.Hani, ChineseConversionVariant.Hant, "jp2t")]
    public async Task OpenCCTest(ChineseConversionVariant from, ChineseConversionVariant to, string caseSetName)
    {
        var converter = await ChineseConversionPresets.GetConverterAsync(from, to);
        var caseIndex = 1;
        foreach (var (input, expected) in OpenCCUtils.ReadTestCases(caseSetName))
        {
            Output.WriteLine("Case #{0} ----------", caseIndex);
            Output.WriteLine("    {0}", input);
            Output.WriteLine(" -> {0}", expected);
            var actual = converter.Convert(input);
            Assert.Equal(expected, actual);
            caseIndex++;
        }
    }

}
