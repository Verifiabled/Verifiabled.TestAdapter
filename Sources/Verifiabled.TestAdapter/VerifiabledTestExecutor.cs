using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Verifiabled.TestAdapter.CaseDiscovery;
using Verifiabled.TestAdapter.CaseExecution;

namespace Verifiabled.TestAdapter
{
    [ExtensionUri(VerifiabledExecutorConstants.UriString)]
    public sealed class VerifiabledTestExecutor : ITestExecutor
    {
        private ICaseDiscoverer CasesDiscoverer { get; }
        private ICaseExecution CaseExecution { get; }
        private CancellationTokenSource CancellationTokenSource { get; set; }

        public VerifiabledTestExecutor() : this(new DefaultCaseDiscoverer(), new DefaultCaseExecution())
        { }

        internal VerifiabledTestExecutor(ICaseDiscoverer casesDiscoverer, ICaseExecution caseExecution)
        {
            CasesDiscoverer = casesDiscoverer;
            CaseExecution = caseExecution;
            CancellationTokenSource = new();
        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource = new();
        }

        public void RunTests(IEnumerable<TestCase>? tests, IRunContext? _, IFrameworkHandle? frameworkHandle)
        {
            if (frameworkHandle == null)
                return;

            if (tests == null)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, $"Empty {nameof(tests)}");
                return;
            }

            RunTestsExecutionLogic(tests, frameworkHandle);
        }

        public void RunTests(IEnumerable<string>? sources, IRunContext? _, IFrameworkHandle? frameworkHandle)
        {
            if (frameworkHandle == null)
                return;

            if (sources == null)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, $"Empty {nameof(sources)}");
                return;
            }

            var discoveredCases = sources.SelectMany(source => CasesDiscoverer.Explore(source));

            frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Cases found: {discoveredCases.Count()}");

            RunTestsExecutionLogic(discoveredCases, frameworkHandle);
        }

        private void RunTestsExecutionLogic(IEnumerable<TestCase> tests, IFrameworkHandle frameworkHandle)
        {
            Cancel();

            foreach (var test in tests)
            {
                frameworkHandle.RecordStart(test);
                var testResult = CaseExecution.Execute(test, CancellationTokenSource.Token);
                frameworkHandle.RecordResult(testResult);
                frameworkHandle.RecordEnd(test, testResult.Outcome);
            }
        }
    }
}
