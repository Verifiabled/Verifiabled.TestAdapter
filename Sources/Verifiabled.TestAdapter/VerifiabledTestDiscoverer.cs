using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.ComponentModel;
using Verifiabled.TestAdapter.CaseDiscovery;
using Verifiabled.TestAdapter.Logger;

namespace Verifiabled.TestAdapter
{
    [DefaultExecutorUri(VerifiabledExecutorConstants.UriString)]
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [Category("managed")]
    public class VerifiabledTestDiscoverer : ITestDiscoverer
    {
        private ICaseDiscoverer CasesDiscoverer { get; }

        public VerifiabledTestDiscoverer() : this(new DefaultCaseDiscoverer())
        { }

        internal VerifiabledTestDiscoverer(ICaseDiscoverer casesDiscoverer)
        {
            CasesDiscoverer = casesDiscoverer;
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            var messageLogger = new MessageLoggerLogger(logger);

            foreach (var source in sources)
            {
                logger.SendMessage(TestMessageLevel.Informational, $"Container found: {source}");

                var discoveredCases = CasesDiscoverer.Explore(source, messageLogger);

                logger.SendMessage(TestMessageLevel.Informational, $"Cases found: {discoveredCases.Count()}");

                foreach (var discoveredCase in discoveredCases)
                    discoverySink.SendTestCase(discoveredCase);
            }
        }
    }
}
