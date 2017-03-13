using System;
using Interfaces.Destination.Filename;

namespace Controllers.Destination.Filename
{
    public class LogFilenameDelegate : IGetFilenameDelegate
    {
        private const string logTimestampFormat = "yyyyMMdd-HHmmss";

        public string GetFilename(string source = null)
        {
            var timestamp = DateTime.Now.ToString(logTimestampFormat);
            return $"log-{timestamp}.json";
        }
    }
}
