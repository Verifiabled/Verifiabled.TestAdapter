using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Verifiabled.TestAdapter.Logger;

namespace Verifiabled.TestAdapter.CaseDiscovery
{
    internal interface ICaseDiscoverer
    {
        IEnumerable<TestCase> Explore(string source, ILogger logger);
    }
}
