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

                var assemblyName = assembly.GetName().Name;

                if(assemblyName == null)
                {
                    logger.Error("Assembly name not retrieved");
                    return testCases;
                }

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

                        var fullyQualifiedName = OriginPropagator.Propagate(assemblyName, type.Namespace, type.Name, method.Name);
                        var testCase = new TestCase(fullyQualifiedName, VerifiabledExecutorConstants.Uri, source);

                        // add the symbol reader, read .pdb and point test case towards source file and line

                        testCases.Add(testCase);
                    }
                }
            }

            catch(Exception exception)
            {
                logger.Error($"{exception.GetType().Name}: {exception.Message}");
            }

            return testCases;
        }
    }
}
