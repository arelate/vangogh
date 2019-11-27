using System.Threading.Tasks;

using Interfaces.Delegates.Constrain;
using Interfaces.Delegates.Format;

using Interfaces.Status;

using Attributes;

using Models.Units;

namespace Delegates.Constrain
{
    public class ConstrainExecutionAsyncDelegate : IConstrainAsyncDelegate<int>
    {
        readonly IStatusController statusController;
        readonly IFormatDelegate<long, string> formatSecondsDelegate;

        [Dependencies(
            "Controllers.Status.StatusController,Controllers",
            "Delegates.Format.Numbers.FormatSecondsDelegate,Delegates")]
        public ConstrainExecutionAsyncDelegate(
            IStatusController statusController,
            IFormatDelegate<long, string> formatSecondsDelegate)
        {
            this.statusController = statusController;
            this.formatSecondsDelegate = formatSecondsDelegate;
        }

        public async Task ConstrainAsync(int delaySeconds, IStatus status)
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
