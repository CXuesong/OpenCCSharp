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
        var s2t = await OpenCCUtils.CreateConverterFromAsync("s2t.json");
        Assert.Equal("�����D�Q�yԇ", s2t.Convert("������Ϣת������"));
    }

}
