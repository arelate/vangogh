using System.Threading;

using Interfaces.Throttle;
using Interfaces.TaskStatus;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.Throttle
{
    public class ThrottleController : IThrottleController
    {
        private ITaskStatusController taskStatusController;
        IFormattingController secondsFormattingController;
        private int delaySeconds;
        private long threshold;

        public ThrottleController(
            ITaskStatusController taskStatusController,
            IFormattingController secondsFormattingController,
            long threshold,
            int delaySeconds = 60 * 2)
        {
            this.taskStatusController = taskStatusController;
            this.secondsFormattingController = secondsFormattingController;
            this.threshold = threshold;
            this.delaySeconds = delaySeconds;
        }

        public void Throttle(ITaskStatus taskStatus)
        {
            var throttleTask = taskStatusController.Create(
                taskStatus,
                $"Wait {secondsFormattingController.Format(delaySeconds)} before next iteration");
            for (var ii = 0; ii < delaySeconds; ii++)
            {
                Thread.Sleep(1000);
                taskStatusController.UpdateProgress(throttleTask, ii + 1, delaySeconds, "Countdown", TimeUnits.Seconds);
            }
            taskStatusController.Complete(throttleTask);
        }

        public long Threshold
        {
            get { return threshold; }
        }
    }
}
