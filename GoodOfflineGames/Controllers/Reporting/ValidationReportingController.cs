using Interfaces.Formatting;
using Interfaces.Reporting;

namespace Controllers.Reporting
{
    public class ValidationReportingController : IReportProgressDelegate
    {
        private IFormattingController bytesFormattingController;
        private ITaskReportingController taskReportingController;

        public ValidationReportingController(
            IFormattingController bytesFormattingController,
            ITaskReportingController taskReportingController)
        {
            this.bytesFormattingController = bytesFormattingController;
            this.taskReportingController = taskReportingController;
        }

        public void ReportProgress(long value, long maxValue, LongToStringFormattingDelegate formattingDelegate = null)
        {
            taskReportingController.ReportProgress(value, maxValue, bytesFormattingController.Format);
        }
    }
}
