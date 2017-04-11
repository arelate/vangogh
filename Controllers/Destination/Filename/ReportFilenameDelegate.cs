using System;
using Interfaces.Destination.Filename;

namespace Controllers.Destination.Filename
{
    public class ReportFilenameDelegate : IGetFilenameDelegate
    {
        private const string reportTimestampFormat = "yyyyMMdd-HHmmss";

        public string GetFilename(string source = null)
        {
            var timestamp = DateTime.Now.ToString(reportTimestampFormat);
            return $"report-{timestamp}.txt";
        }
    }
}
