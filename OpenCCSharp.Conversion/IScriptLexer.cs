using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Conversion;

public interface IScriptLexer
{

    IEnumerable<(int Start, int Length)> EnumSegments(ReadOnlySpan<char> content);

}
