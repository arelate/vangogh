using System.Threading;

using Interfaces.Reporting;
using Interfaces.Throttle;
using Interfaces.Formatting;

namespace Controllers.Throttle
{
    public class ThrottleController : IThrottleController
    {
        private ITaskReportingController taskReportingController;
        private IFormattingController secondsFormattingController;
        private int delayMilliseconds; // default is 2 minutes


        public ThrottleController(
            ITaskReportingController taskReportingController,
            IFormattingController secondsFormattingController,
            int delayMilliseconds = 1000 * 60 * 2) 
        {
            this.taskReportingController = taskReportingController;
            this.secondsFormattingController = secondsFormattingController;
            this.delayMilliseconds = delayMilliseconds;
        }

        public void Throttle()
        {
            taskReportingController?.StartTask(string.Format(
                "Throttle current activity by {0}",
                secondsFormattingController?.Format(delayMilliseconds / 1000)));
            Thread.Sleep(delayMilliseconds);
            taskReportingController?.CompleteTask();
        }
    }
}
