namespace Verifiabled.TestAdapter
{
    internal static class VerifiabledExecutorConstants
    {
        internal const string UriString = $"executor://{nameof(VerifiabledTestExecutor)}";
        internal static readonly Uri Uri = new(UriString);
    }
}
