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
        var s2tw = await OpenCCUtils.CreateConverterFromAsync("s2tw.json");
        Assert.Equal("�yԇ", s2tw.Convert("����"));
    }

}
