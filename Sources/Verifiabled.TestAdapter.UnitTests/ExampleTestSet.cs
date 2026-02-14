namespace Verifiabled.TestAdapter.UnitTests
{
    public sealed class ExampleTestSet
    {
        [Case]
        public void ExamplePassingTestCase()
        {
            var expected = true;

            Assert.IsTrue(expected);
        }
    }
}
