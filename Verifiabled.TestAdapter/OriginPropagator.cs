namespace Verifiabled.TestAdapter
{
    internal static class OriginPropagator
    {
        private const char Separator = '/';

        internal static string Propagate(string assemblyName, string className, string methodName) => string.Join(Separator, assemblyName, className, methodName);

        internal static (string assemblyName, string className, string methodName) Depropagate(string payload)
        {
            var parts = payload.Split(Separator);
            return (parts[0], parts[1], parts[2]);
        }
    }
}
