namespace Verifiabled.TestAdapter
{
    internal static class OriginPropagator
    {
        private const char Separator = '.';

        internal static string Propagate(string typeFullName, string methodName) => $"{typeFullName}{Separator}{methodName}";

        internal static (string typeFullName, string methodName) Depropagate(string fullyQualifiedName)
        {
            var lastSeparatorIndex = fullyQualifiedName.LastIndexOf(Separator);
            
            if (lastSeparatorIndex < 0)
                return (string.Empty, fullyQualifiedName);

            var typeFullName = fullyQualifiedName.Substring(0, lastSeparatorIndex);
            var methodName = fullyQualifiedName.Substring(lastSeparatorIndex + 1);
            
            return (typeFullName, methodName);
        }
    }
}
