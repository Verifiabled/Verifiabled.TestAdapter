using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Reflection;

namespace Verifiabled.TestAdapter.CaseDiscovery
{
    internal class DefaultCaseDiscoverer : ICaseDiscoverer
    {
        public IEnumerable<TestCase> Explore(string source)
        {
            var assembly = Assembly.LoadFile(source);

            if (!assembly.GetReferencedAssemblies().Any(assembly => assembly.Name == nameof(Verifiabled)))
                yield break;

            foreach(var type in assembly.GetTypes())
            {
                foreach(var method in type.GetMethods())
                {
                    var attribute = method.GetCustomAttribute<CaseAttribute>();

                    if (attribute == null)
                        break;

                    yield return new TestCase(OriginPropagator.Propagate(assembly.GetName().FullName, type.Name, method.Name), VerifiabledExecutorConstants.Uri, source);
                }
            }
        }
    }
}
