using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Verifiabled.TestAdapter.Logger;

namespace Verifiabled.TestAdapter.CaseExecution
{
    internal sealed class DefaultCaseExecution : ICaseExecution
    {
        public TestResult Execute(TestCase testCase, ILogger logger, CancellationToken cancellationToken)
        {
            var testResult = new TestResult(testCase);
            testResult.StartTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.Information("Cancellation requested");
                    testResult.Outcome = TestOutcome.None;
                    return testResult;
                }

                ExecuteTest(testCase, testResult, logger, cancellationToken);
            }

            catch (Exception exception)
            {
                HandleException(exception, testResult);
            }

            finally
            {
                stopwatch.Stop();
                testResult.Duration = stopwatch.Elapsed;
                testResult.EndTime = DateTimeOffset.UtcNow;
            }

            return testResult;
        }

        private static void ExecuteTest(TestCase testCase, TestResult testResult, ILogger logger, CancellationToken cancellationToken)
        {
            var (typeFullName, methodName) = OriginPropagator.Depropagate(testCase.FullyQualifiedName);

            var assembly = Assembly.LoadFrom(testCase.Source);

            if (assembly == null)
            {
                logger.Error($"Assembly not found: {testCase.Source}");
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            var type = assembly.GetType(typeFullName);

            if (type == null)
            {
                logger.Error($"Type not found for test case: {testCase.FullyQualifiedName}");
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            if (type.IsNotPublic)
            {
                logger.Error($"Type is not public: {type.FullName}");
                testResult.Outcome = TestOutcome.Failed;
                testResult.ErrorMessage = "Test case could not be executed because the class containing this case is not public";
                return;
            }

            var method = type.GetMethod(methodName);

            if (method == null)
            {
                logger.Error($"Method not found: {methodName}");
                testResult.Outcome = TestOutcome.NotFound;
                return;
            }

            if (!method.IsPublic)
            {
                logger.Error($"Method is not public: {methodName}");
                testResult.Outcome = TestOutcome.Failed;
                testResult.ErrorMessage = "Test case could not be executed because the method containing this case is not public";
                return;
            }

            var voidMethod = method.ReturnType == typeof(void);
            var taskMethod = method.ReturnType == typeof(Task);

            if (!voidMethod && !taskMethod)
            {
                logger.Error($"Method has unsupported return type: {method.ReturnType}");
                testResult.Outcome = TestOutcome.Failed;
                testResult.ErrorMessage = "Test case could not be executed because the method containing this case has unsupported return type. Only void and Task are supported";
                return;
            }

            if (method.GetParameters().Length > 0)
            {
                logger.Error($"Method has unsupported parameters: {methodName}");
                testResult.Outcome = TestOutcome.Failed;
                testResult.ErrorMessage = "Test case could not be executed because the method containing this case has unsupported parameters. Only parameterless methods are supported";
                return;
            }

            var classIsStatic = type.IsAbstract && type.IsSealed;
            var instance = classIsStatic ? null : Activator.CreateInstance(type);

            if (instance == null && !classIsStatic)
            {
                logger.Error($"Instance was not created");
                testResult.Outcome = TestOutcome.Failed;
                testResult.ErrorMessage = "Test case could not be executed because class containing this case could not be instantiated";
                return;
            }

            if (voidMethod)
                RunSyncMethod(method, instance, cancellationToken);
            else
                RunAsyncMethod(method, instance, cancellationToken);

            testResult.Outcome = TestOutcome.Passed;
        }

        private static void RunSyncMethod(MethodInfo method, object? instance, CancellationToken cancellationToken)
        {
            using var caseTask = Task.Factory.StartNew(() => method.Invoke(instance, null), cancellationToken);
            caseTask.Wait(cancellationToken);
        }

        private static void RunAsyncMethod(MethodInfo method, object? instance, CancellationToken cancellationToken)
        {
            using var caseTask = (Task)method.Invoke(instance, null)!;
            caseTask.Wait(cancellationToken);
        }

        private static void HandleException(Exception exception, TestResult testResult)
        {
            testResult.Outcome = TestOutcome.Failed;
            testResult.ErrorMessage = exception.Message;
            testResult.ErrorStackTrace = exception.StackTrace;
        }
    }
}
