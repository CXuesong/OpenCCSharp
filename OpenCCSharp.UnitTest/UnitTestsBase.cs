using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
