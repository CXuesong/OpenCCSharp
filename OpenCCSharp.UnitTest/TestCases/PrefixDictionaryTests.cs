using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCCSharp.Conversion;
using Xunit;
using Xunit.Abstractions;

namespace OpenCCSharp.UnitTest.TestCases
{
    public class PrefixDictionaryTests : UnitTestsBase
    {
        /// <inheritdoc />
        public PrefixDictionaryTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SortedStringPrefixDictionaryTest()
        {
            var dict = new SortedStringPrefixDictionary
            {
                { "B", "b" },
                { "A", "a" },
                { "C", "c" },
                { "D", "d" },
                { "Z", "z" },
                { "E", "e" },
                { "F", "f" },
            };
            // Sanity check.
            Assert.NotEmpty(dict);
            Assert.All(dict, p =>
            {
                Assert.Equal(p.Value, dict[p.Key]);
                Assert.True(dict.TryGetValue(p.Key, out var v));
                Assert.Equal(p.Value, v);

                Assert.True(dict.TryGetLongestPrefixingKey(p.Key.Span, out var k));
                Assert.Equal(p.Key, k);
            });
        }
    }
}
