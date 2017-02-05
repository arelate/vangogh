using System;
using Interfaces.Destination.Filename;

namespace Controllers.Destination.Filename
{
    public class LogFilenameDelegate : IGetFilenameDelegate
    {
        private const string logFilenameTemplate = "log-{0}.json";
        private const string logTimestampFormat = "yyyyMMdd-HHmmss";

        public string GetFilename(string source = null)
        {
            return
                string.Format(
                    logFilenameTemplate,
                    DateTime.Now.ToString(logTimestampFormat));
        }
    }
}
