using System;

using Interfaces.Console;
using Interfaces.Formatting;
using Interfaces.Reporting;

namespace Controllers.Reporting
{
    public class DownloadReportingController : IDownloadReportingController
    {
        private IConsoleController consoleController;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;
        private DateTime startTimestamp;
        private DateTime lastReportedTimestamp = DateTime.MinValue;
        private const string reportFormat = "\r{0:P1} at {1:D}/s. ETA: {2:D}         ";
        private const string downloadComplete = "\rFile was successfully downloaded.                  ";

        private const int throttleMilliseconds = 1000;

        public DownloadReportingController(
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController,
            IConsoleController consoleController)
        {
            this.consoleController = consoleController;

            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public void StartTask(string name, params object[] values)
        {
            startTimestamp = DateTime.Now;
            lastReportedTimestamp = DateTime.Now;
        }

        public void ReportProgress(long currentValue, long? maxValue, LongToStringFormattingDelegate formattingDelegate = null)
        {
            // throttle updates to once in throttleMillisecond
            if ((DateTime.Now - lastReportedTimestamp).TotalMilliseconds < throttleMilliseconds) return;

            // calculate percent completion
            var percent = (double)currentValue / maxValue;
            var elapsed = DateTime.Now - startTimestamp;
            var bytesPerSecond = currentValue / elapsed.TotalSeconds;
            var remainingSeconds = (maxValue - currentValue) / bytesPerSecond;

            var formattedBytesPerSecond = bytesFormattingController.Format((long)bytesPerSecond);
            var formattedRemainingSeconds = secondsFormattingController.Format((long)remainingSeconds);

            consoleController.Write(reportFormat, MessageType.Progress, percent, formattedBytesPerSecond, formattedRemainingSeconds);

            lastReportedTimestamp = DateTime.Now;
        }

        public void CompleteTask()
        {
            consoleController.WriteLine(downloadComplete, MessageType.Success);
        }
    }

}
