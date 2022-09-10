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

                var (len, value) = dict.TryGetLongestPrefixingKey(p.Key.Span);
                Assert.Equal(p.Key.Length, len);
                Assert.Equal(p.Value, value);
            });
        }
    }
}
