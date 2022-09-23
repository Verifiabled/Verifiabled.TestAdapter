using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Verifiabled.TestAdapter.CaseDiscovery
{
    internal interface ICaseDiscoverer
    {
        IEnumerable<TestCase> Explore(string source, Action<string> logger);
    }
}
