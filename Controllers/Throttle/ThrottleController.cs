using System.Threading.Tasks;

using Interfaces.Throttle;
using Interfaces.Status;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.Throttle
{
    public class ThrottleController : IThrottleController
    {
        private IStatusController statusController;
        IFormattingController secondsFormattingController;

        public ThrottleController(
            IStatusController statusController,
            IFormattingController secondsFormattingController)
        {
            this.statusController = statusController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public async Task ThrottleAsync(int delaySeconds, IStatus status)
        {
            var throttleTask = statusController.Create(
                status,
                $"Sleeping {secondsFormattingController.Format(delaySeconds)} before next operation");

            for (var ii = 0; ii < delaySeconds; ii++)
            {
                await Task.Delay(1000);
                statusController.UpdateProgress(
                    throttleTask, 
                    ii + 1, 
                    delaySeconds, 
                    "Countdown", 
                    TimeUnits.Seconds);
            }

            statusController.Complete(throttleTask);
        }
    }
}
