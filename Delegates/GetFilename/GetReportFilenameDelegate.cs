using System;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetFilename
{
    public class GetReportFilenameDelegate : IGetFilenameDelegate
    {
        private const string reportTimestampFormat = "yyyyMMdd-HHmmss";

        public string GetFilename(string source = null)
        {
            var timestamp = DateTime.Now.ToString(reportTimestampFormat);
            return $"report-{timestamp}.txt";
        }
    }
}
