using System;
using Interfaces.Destination;

namespace Controllers.Destination
{
    public class LogsDestinationController : IDestinationController
    {
        private const string logFilenameTemplate = "log-{0}.js";
        private const string logTimestampFormat = "yyyyMMdd-HHmmss";

        public string GetDirectory(string source)
        {
            return "logs";
        }

        public string GetFilename(string source)
        {
            return
                string.Format(
                    logFilenameTemplate,
                    DateTime.Now.ToString(logTimestampFormat));
        }
    }
}
