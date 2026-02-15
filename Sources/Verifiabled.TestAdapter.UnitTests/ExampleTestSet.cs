using System.Threading.Tasks;

namespace Verifiabled.TestAdapter.UnitTests
{
    public sealed class ExampleTestSet
    {
        [Case]
        public void ExamplePassingVoidSyncCase()
        {
            var expected = true;

            Assert.IsTrue(expected);
        }

        [Case]
        public async Task ExamplePassingVoidAsyncCase()
        {
            await Task.CompletedTask;
        }

        [Case]
        public static void ExamplePassingStaticVoidSyncCase()
        {
            var expected = true;

            Assert.IsTrue(expected);
        }

        [Case]
        public static async Task ExamplePassingStaticVoidAsyncCase()
        {
            await Task.CompletedTask;
        }
    }
}
