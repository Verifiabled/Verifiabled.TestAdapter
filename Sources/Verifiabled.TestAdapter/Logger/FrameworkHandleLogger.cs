using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Verifiabled.TestAdapter.Logger
{
    internal class FrameworkHandleLogger : ILogger
    {
        private IFrameworkHandle FrameworkHandle { get; }

        public FrameworkHandleLogger(IFrameworkHandle frameworkHandle)
        {
            FrameworkHandle = frameworkHandle;
        }

        public void Error(string message) => FrameworkHandle.SendMessage(TestMessageLevel.Error, message);

        public void Information(string message) => FrameworkHandle.SendMessage(TestMessageLevel.Informational, message);

        public void Warning(string message) => FrameworkHandle.SendMessage(TestMessageLevel.Warning, message);
    }
}
