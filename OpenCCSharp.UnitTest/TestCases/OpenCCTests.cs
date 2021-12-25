using System.Threading.Tasks;
using OpenCCSharp.Conversion;
using Xunit;
using Xunit.Abstractions;

namespace OpenCCSharp.UnitTest.TestCases;

/// <summary>
/// Test cases ported from OpenCC C++ project.
/// </summary>
public class OpenCCTests : UnitTestsBase
{

    /// <inheritdoc />
    public OpenCCTests(ITestOutputHelper output) : base(output)
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

}
