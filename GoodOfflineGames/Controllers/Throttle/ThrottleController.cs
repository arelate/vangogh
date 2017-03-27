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

        public ThrottleController(
            ITaskStatusController taskStatusController,
            IFormattingController secondsFormattingController,
            int delaySeconds)
        {
            this.taskStatusController = taskStatusController;
            this.secondsFormattingController = secondsFormattingController;
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
    }
}
