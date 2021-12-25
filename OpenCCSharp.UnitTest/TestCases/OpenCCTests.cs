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
        AssertConversionPair(s2t, t2s, "����", "�{ԇ");
        AssertConversionPair(s2twp, tw2sp, "����", "���e");
        AssertConversionPair(s2t, t2s, "�鿴���ķ���ת����Ԫ���Խ�����Ե��Կ��ܳ��ֵĴ����߼�����", "�鿴���ķ����D�Q��Ԫ�yԇ�Y�������{ԇ���ܳ��F�Ĵ��a߉݋�e�`��");
        AssertConversionPair(s2twp, tw2sp, "�鿴���ķ���ת����Ԫ���Խ�����Ե��Կ��ܳ��ֵĴ����߼�����", "�zҕ���ķ����D�Q��Ԫ�yԇ�Y�����Գ��e���ܳ��F�ĳ�ʽ�a߉݋�e�`��");
    }

}
