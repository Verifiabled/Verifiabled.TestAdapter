using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Verifiabled.TestAdapter.Logger;

namespace Verifiabled.TestAdapter.CaseExecution
{
    internal interface ICaseExecution
    {
        TestResult Execute(TestCase testCase, ILogger logger, CancellationToken cancellationToken);
    }
}
