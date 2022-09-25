using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Reflection;
using Verifiabled.TestAdapter.Logger;

namespace Verifiabled.TestAdapter.CaseDiscovery
{
    internal class DefaultCaseDiscoverer : ICaseDiscoverer
    {
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
                    logger.Information($"Type explored {type.Name}");

                    foreach (var method in type.GetMethods())
                    {
                        logger.Information($"Method explored {method.Name}");

                        var attribute = method.GetCustomAttributes().FirstOrDefault(att => att.GetType().Name == nameof(CaseAttribute));

                        if (attribute == null)
                        {
                            logger.Information($"No CaseAttribute");
                            break;
                        }

                        testCases.Add(new TestCase(OriginPropagator.Propagate(assemblyName, type.Name, method.Name), VerifiabledExecutorConstants.Uri, source));
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
