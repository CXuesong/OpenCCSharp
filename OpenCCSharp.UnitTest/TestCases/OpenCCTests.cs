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
        Assert.Equal("�{ԇ", s2t.Convert("����"));
        Assert.Equal("�{ԇ", s2twp.Convert("����"));
        Assert.Equal("�鿴���ķ����D�Q��Ԫ�yԇ�Y�������{ԇ���ܳ��F�Ĵ��a߉݋�e�`��", s2t.Convert("�鿴���ķ���ת����Ԫ���Խ�����Ե��Կ��ܳ��ֵĴ����߼�����"));
        Assert.Equal("�鿴���ķ����D�Q��Ԫ�yԇ�Y�������{ԇ���ܳ��F�Ĵ��a߉݋�e�`��", s2twp.Convert("�鿴���ķ���ת����Ԫ���Խ�����Ե��Կ��ܳ��ֵĴ����߼�����"));
    }

}
