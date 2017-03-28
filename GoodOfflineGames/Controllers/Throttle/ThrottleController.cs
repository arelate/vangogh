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

        public ThrottleController(
            ITaskStatusController taskStatusController,
            IFormattingController secondsFormattingController)
        {
            this.taskStatusController = taskStatusController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public void Throttle(int delaySeconds, ITaskStatus taskStatus)
        {
            var throttleTask = taskStatusController.Create(
                taskStatus,
                $"Sleeping {secondsFormattingController.Format(delaySeconds)} before next operation");

            for (var ii = 0; ii < delaySeconds; ii++)
            {
                Thread.Sleep(1000);
                taskStatusController.UpdateProgress(throttleTask, ii + 1, delaySeconds, "Countdown", TimeUnits.Seconds);
            }

            taskStatusController.Complete(throttleTask);
        }
    }
}
