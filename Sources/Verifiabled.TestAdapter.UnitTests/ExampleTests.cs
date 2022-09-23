namespace Verifiabled.TestAdapter.UnitTests
{
    public sealed class ExampleTests
    {
        [Case]
        public void ExamplePass()
        {
            var haystack = new[] { 1, 2, 3, 5 };

            Assert.That(haystack.AsEnumerable()).Contains(2);
        }

        [Case]
        public void ExampleFail()
        {
            var haystack = new[] { 1, 2, 3, 5 };

            Assert.That(haystack.AsEnumerable()).Contains(9);
        }
    }
}