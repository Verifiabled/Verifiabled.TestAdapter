using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Reflection;

namespace Verifiabled.TestAdapter.CaseDiscovery
{
    internal class DefaultCaseDiscoverer : ICaseDiscoverer
    {
        public IEnumerable<TestCase> Explore(string source, Action<string> logger)
        {
            logger("Explore");

            var testCases = new List<TestCase>();

            try
            {
                var assembly = Assembly.LoadFile(source);

                if (assembly == null)
                {
                    logger($"Assembly not found: {source}");
                    return testCases;
                }

                var assemblyName = assembly.GetName().Name;

                if(assemblyName == null)
                {
                    logger($"Assembly name not retrieved");
                    return testCases;
                }

                foreach (var type in assembly.GetTypes())
                {
                    logger($"Type explored {type.Name}");

                    foreach (var method in type.GetMethods())
                    {
                        logger($"Method explored {method.Name}");

                        var attribute = method.GetCustomAttributes().FirstOrDefault(att => att.GetType().Name == nameof(CaseAttribute));

                        if (attribute == null)
                        {
                            logger($"No CaseAttribute");
                            break;
                        }

                        testCases.Add(new TestCase(OriginPropagator.Propagate(assemblyName, type.Name, method.Name), VerifiabledExecutorConstants.Uri, source));
                    }
                }
            }

            catch(Exception exception)
            {
                logger($"{exception.GetType().Name}: {exception.Message}");
            }

            return testCases;
        }
    }
}
