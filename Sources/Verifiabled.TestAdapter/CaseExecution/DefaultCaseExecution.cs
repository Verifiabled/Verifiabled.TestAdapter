using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Verifiabled.Constraints;
using Verifiabled.TestAdapter.Logger;

namespace Verifiabled.TestAdapter.CaseExecution
{
    internal class DefaultCaseExecution : ICaseExecution
    {
        public TestResult Execute(TestCase testCase, ILogger logger, CancellationToken cancellationToken)
        {
            var testResult = new TestResult(testCase);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.Information("Cancellation requested");
                    testResult.Outcome = TestOutcome.None;
                    return testResult;
                }

                ExecuteTest(testCase, testResult, logger);
            }

            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    HandleException(exception.InnerException, testResult);

                else
                    HandleException(exception, testResult);
            }

            finally
            {
                stopwatch.Stop();
                testResult.Duration = stopwatch.Elapsed;
            }

            return testResult;
        }

        private static void ExecuteTest(TestCase testCase, TestResult testResult, ILogger logger)
        {
            (string assemblyName, string className, string methodName) = OriginPropagator.Depropagate(testCase.FullyQualifiedName);

            var assembly = Assembly.Load(assemblyName);

            if (assembly == null)
            {
                logger.Error($"Assembly not found: {assemblyName}");
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);

            if (type == null)
            {
                logger.Error($"Type not found: {className}");
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            var method = type.GetMethod(methodName);

            if (method == null)
            {
                logger.Error($"Method not found: {methodName}");
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            var instance = Activator.CreateInstance(type);

            if (instance == null)
            {
                logger.Error($"Instance was not created");
                testResult.Outcome = TestOutcome.Failed;
                return;
            }

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

        private static void HandleException(Exception exception, TestResult testResult)
        {
            testResult.Outcome = TestOutcome.Failed;
            testResult.ErrorMessage = exception.Message;
            testResult.ErrorStackTrace = exception.StackTrace;
        }
    }
}
