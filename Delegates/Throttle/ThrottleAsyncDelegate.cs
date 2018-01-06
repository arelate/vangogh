using System.Threading.Tasks;

using Interfaces.Delegates.Throttle;

using Interfaces.Status;
using Interfaces.Formatting;

using Models.Units;

namespace Delegates.Throttle
{
    public class ThrottleAsyncDelegate : IThrottleAsyncDelegate
    {
        private IStatusController statusController;
        IFormattingController secondsFormattingController;

        public ThrottleAsyncDelegate(
            IStatusController statusController,
            IFormattingController secondsFormattingController)
        {
            this.statusController = statusController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public async Task ThrottleAsync(int delaySeconds, IStatus status)
        {
            var throttleTask = await statusController.CreateAsync(
                status,
                $"Sleeping {secondsFormattingController.Format(delaySeconds)} before next operation");

            for (var ii = 0; ii < delaySeconds; ii++)
            {
                await Task.Delay(1000);
                await statusController.UpdateProgressAsync(
                    throttleTask, 
                    ii + 1, 
                    delaySeconds, 
                    "Countdown", 
                    TimeUnits.Seconds);
            }

            await statusController.CompleteAsync(throttleTask);
        }
    }
}
