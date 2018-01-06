using System.Threading.Tasks;

using Interfaces.Delegates.Throttle;
using Interfaces.Delegates.Format;

using Interfaces.Status;

using Models.Units;

namespace Delegates.Throttle
{
    public class ThrottleAsyncDelegate : IThrottleAsyncDelegate
    {
        private IStatusController statusController;
        IFormatDelegate<long, string> formatSecondsDelegate;

        public ThrottleAsyncDelegate(
            IStatusController statusController,
            IFormatDelegate<long, string> formatSecondsDelegate)
        {
            this.statusController = statusController;
            this.formatSecondsDelegate = formatSecondsDelegate;
        }

        public async Task ThrottleAsync(int delaySeconds, IStatus status)
        {
            var throttleTask = await statusController.CreateAsync(
                status,
                $"Sleeping {formatSecondsDelegate.Format(delaySeconds)} before next operation");

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
