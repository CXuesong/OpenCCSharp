using System;
using Xunit.Abstractions;

namespace OpenCCSharp.UnitTest;

public class UnitTestsBase
{
    public UnitTestsBase(ITestOutputHelper output)
    {
        Output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public ITestOutputHelper Output { get; }
}
