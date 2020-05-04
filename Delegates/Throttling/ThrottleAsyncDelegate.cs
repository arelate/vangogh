using System.Threading.Tasks;
using Interfaces.Delegates.Throttling;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Activities;
using Attributes;
using Delegates.Conversions.Units;
using Delegates.Activities;

namespace Delegates.Throttling
{
    public class ThrottleAsyncDelegate : IThrottleAsyncDelegate<int>
    {
        private readonly IConvertDelegate<long, string> convertSecondsToStringDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ConvertSecondsToStringDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public ThrottleAsyncDelegate(
            IConvertDelegate<long, string> convertSecondsToStringDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.convertSecondsToStringDelegate = convertSecondsToStringDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ThrottleAsync(int delaySeconds)
        {
            startDelegate.Start(
                $"Sleeping {convertSecondsToStringDelegate.Convert(delaySeconds)} before next operation");

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