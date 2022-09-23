using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Verifiabled.Constraints;

namespace Verifiabled.TestAdapter.CaseExecution
{
    internal class DefaultCaseExecution : ICaseExecution
    {
        public TestResult Execute(TestCase testCase, CancellationToken cancellationToken)
        {
            var testResult = new TestResult(testCase);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    testResult.Outcome = TestOutcome.None;
                    return testResult;
                }

                ExecuteTest(testCase, testResult);
            }

            catch (Exception exception)
            {
                testResult.Outcome = TestOutcome.Failed;
                testResult.ErrorMessage = exception.Message;
                testResult.ErrorStackTrace = exception.StackTrace;
            }

            finally
            {
                stopwatch.Stop();
                testResult.Duration = stopwatch.Elapsed;
            }

            return testResult;
        }

        private static void ExecuteTest(TestCase testCase, TestResult testResult)
        {
            (string assemblyName, string className, string methodName) = OriginPropagator.Depropagate(testCase.FullyQualifiedName);

            var assembly = Assembly.Load(assemblyName);

            if(assembly == null)
            {
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            var type = assembly.GetType(className);

            if(type == null)
            {
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            var method = type.GetMethod(methodName);

            if(method == null)
            {
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            var instance = type.Assembly.CreateInstance(className);

            var constraintListener = new DefaultConstraintListener();
            GlobalConstraintListenerManager.Add(constraintListener);

            method.Invoke(instance, null);

            var constraints = constraintListener.GetAllContraints();

            if (constraints.Any(constraint => !constraint.IsFulfilled))
            {
                testResult.Outcome = TestOutcome.Failed;
                return;
            }

            testResult.Outcome = TestOutcome.Passed;
        }
    }
}
