namespace Verifiabled.TestAdapter
{
    internal static class OriginPropagator
    {
        private const char Separator = '/';

        internal static string Propagate(string assemblyName, string? namespaceName, string className, string methodName) => string.Join(Separator, assemblyName, namespaceName, className, methodName);

        internal static (string assemblyName, string? namespaceName, string className, string methodName) Depropagate(string payload)
        {
            var parts = payload.Split(Separator);
            return (parts[0], parts[1], parts[2], parts[3]);
        }
    }
}
