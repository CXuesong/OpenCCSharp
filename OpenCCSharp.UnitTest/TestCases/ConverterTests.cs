using System.Threading.Tasks;
using OpenCCSharp.Conversion;
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
        var s2t = await ConversionDefinitionUtils.CreateConverterFromAsync("Hans-Hant.json");
        var t2s = await ConversionDefinitionUtils.CreateConverterFromAsync("Hant-Hans.json");
        var s2twp = await ConversionDefinitionUtils.CreateConverterFromAsync("Hans-TW.json");
        var tw2sp = await ConversionDefinitionUtils.CreateConverterFromAsync("TW-Hans.json");
        AssertConversionPair(s2t, t2s, "调试", "調試");
        AssertConversionPair(s2twp, tw2sp, "调试", "除錯");
        AssertConversionPair(s2t, t2s, "查看中文繁简转换单元测试结果，以调试可能出现的代码逻辑错误。", "查看中文繁簡轉換單元測試結果，以調試可能出現的代碼邏輯錯誤。");
        AssertConversionPair(s2twp, tw2sp, "查看中文繁简转换单元测试结果，以调试可能出现的代码逻辑错误。", "檢視中文繁簡轉換單元測試結果，以除錯可能出現的程式碼邏輯錯誤。");
    }

    /// <summary>
    /// Test cases ported from OpenCC C++ project.
    /// </summary>
    [Theory]
    [InlineData("Hans-Hant.json", "s2t")]
    [InlineData("Hans-HK.json", "s2hk")]
    [InlineData("Hans-TW.json", "s2twp")]
    [InlineData("Hant-Hani.json", "t2jp")]
    [InlineData("Hant-Hans.json", "t2s")]
    [InlineData("HK-Hans.json", "hk2s")]
    [InlineData("TW-Hans.json", "tw2sp")]
    [InlineData("Hani-Hant.json", "jp2t")]
    public async Task OpenCCTest(string conversionDefinitionFileName, string caseSetName)
    {
        var converter = await ConversionDefinitionUtils.CreateConverterFromAsync(conversionDefinitionFileName);
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
