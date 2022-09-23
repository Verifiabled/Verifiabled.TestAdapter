using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Verifiabled.TestAdapter.CaseExecution
{
    internal interface ICaseExecution
    {
        TestResult Execute(TestCase testCase, CancellationToken cancellationToken);
    }
}
