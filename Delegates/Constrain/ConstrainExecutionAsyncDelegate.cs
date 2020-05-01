using System.Threading.Tasks;
using Interfaces.Delegates.Constrain;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Activities;
using Attributes;
using Delegates.Format.Numbers;
using Delegates.Activities;

namespace Delegates.Constrain
{
    public class ConstrainExecutionAsyncDelegate : IConstrainAsyncDelegate<int>
    {
        private readonly IFormatDelegate<long, string> formatSecondsDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(FormatSecondsDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public ConstrainExecutionAsyncDelegate(
            IFormatDelegate<long, string> formatSecondsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.formatSecondsDelegate = formatSecondsDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ConstrainAsync(int delaySeconds)
        {
            startDelegate.Start(
                $"Sleeping {formatSecondsDelegate.Format(delaySeconds)} before next operation");

            for (var ii = 0; ii < delaySeconds; ii++)
                await Task.Delay(1000);
            // await statusController.UpdateProgressAsync(
            //     throttleTask,
            //     ii + 1,
            //     delaySeconds,
            //     "Countdown",
            //     TimeUnits.Seconds);

            completeDelegate.Complete();
        }
    }
}