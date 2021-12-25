using System.Threading.Tasks;
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

    [Fact]
    public async Task Test1()
    {
        var s2t = await ConversionDefinitionUtils.CreateConverterFromAsync("Hans-Hant.json");
        var s2twp = await ConversionDefinitionUtils.CreateConverterFromAsync("Hans-TW.json");
        Assert.Equal("調試", s2t.Convert("调试"));
        Assert.Equal("調試", s2twp.Convert("调试"));
        Assert.Equal("查看中文繁簡轉換單元測試結果，以調試可能出現的代碼邏輯錯誤。", s2t.Convert("查看中文繁简转换单元测试结果，以调试可能出现的代码逻辑错误。"));
        Assert.Equal("查看中文繁簡轉換單元測試結果，以調試可能出現的代碼邏輯錯誤。", s2twp.Convert("查看中文繁简转换单元测试结果，以调试可能出现的代码逻辑错误。"));
    }

}
