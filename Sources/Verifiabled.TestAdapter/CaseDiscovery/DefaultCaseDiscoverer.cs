using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Reflection;
using Verifiabled.TestAdapter.Logger;

namespace Verifiabled.TestAdapter.CaseDiscovery
{
    internal class DefaultCaseDiscoverer : ICaseDiscoverer
    {
        private static readonly string CaseAttributeFullName = typeof(CaseAttribute).FullName ?? string.Empty;

        public IEnumerable<TestCase> Explore(string source, ILogger logger)
        {
            var testCases = new List<TestCase>();

            try
            {
                var assembly = Assembly.LoadFile(source);

                if (assembly == null)
                {
                    logger.Error($"Assembly not found: {source}");
                    return testCases;
                }

                return DiscoverCases(assembly, source, logger);
            }

            catch(Exception exception)
            {
                logger.Error($"{exception.GetType().Name}: {exception.Message}");
            }

            return testCases;
        }

        private IEnumerable<TestCase> DiscoverCases(Assembly assembly, string source, ILogger logger)
        {
            var testCases = new List<TestCase>();
            var assemblyName = assembly.GetName().Name;

            if (assemblyName == null)
            {
                logger.Error("Assembly name not retrieved");
                return testCases;
            }

            using var diaSession = GetDiaSession(source, logger);

            foreach (var type in assembly.GetTypes())
            {
                logger.Information($"Type explored {type.FullName}");

                foreach (var method in type.GetMethods())
                {
                    logger.Information($"Method explored {method.Name}");

                    var attribute = method.GetCustomAttributes().FirstOrDefault(att => att.GetType().FullName == CaseAttributeFullName);

                    if (attribute == null)
                    {
                        logger.Information($"No CaseAttribute");
                        break;
                    }

                    if (type.FullName == null)
                    {
                         logger.Information("Type.FullName is null, skipping");
                         continue;
                    }

                    var fullyQualifiedName = OriginPropagator.Propagate(type.FullName, method.Name);
                    var testCase = new TestCase(fullyQualifiedName, VerifiabledExecutorConstants.Uri, source);
                    testCase.DisplayName = method.Name;

                    if (diaSession != null && type.FullName != null)
                    {
                        var navigationData = diaSession.GetNavigationData(type.FullName, method.Name);

                        if (navigationData != null)
                        {
                            testCase.CodeFilePath = navigationData.FileName;
                            testCase.LineNumber = navigationData.MinLineNumber;
                        }
                    }

                    testCases.Add(testCase);
                }
            }

            return testCases;
        }

        private static DiaSession? GetDiaSession(string source, ILogger logger)
        {
            try
            {
                return new DiaSession(source);
            }
            catch (Exception exception)
            {
                logger.Error($"Failed to create DiaSession for {source}. Error: {exception.Message}");
                return null;
            }
        }
    }
}
