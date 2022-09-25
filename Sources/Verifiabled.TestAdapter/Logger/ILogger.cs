namespace Verifiabled.TestAdapter.Logger
{
    internal interface ILogger
    {
        void Information(string message);
        void Warning(string message);
        void Error(string message);
    }
}
