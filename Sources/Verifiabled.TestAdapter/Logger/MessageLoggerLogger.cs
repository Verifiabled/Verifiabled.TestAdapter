using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Verifiabled.TestAdapter.Logger
{
    internal class MessageLoggerLogger : ILogger
    {
        private IMessageLogger MessageLogger { get; }

        public MessageLoggerLogger(IMessageLogger messageLogger)
        {
            MessageLogger = messageLogger;
        }

        public void Error(string message) => MessageLogger.SendMessage(TestMessageLevel.Error, message);

        public void Information(string message) => MessageLogger.SendMessage(TestMessageLevel.Informational, message);

        public void Warning(string message) => MessageLogger.SendMessage(TestMessageLevel.Warning, message);
    }
}
