namespace Verifiabled.TestAdapter.UnitTests
{
    public sealed class ExampleTestSet
    {
        [Case]
        public void ExamplePassingTestCase()
        {
            var haystack = new[] { 1, 2, 3 };
            var needle = 2;

            Assert.That(haystack).Contains(needle);
        }
    }
}
