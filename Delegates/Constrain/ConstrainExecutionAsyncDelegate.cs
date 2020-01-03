using System.Threading.Tasks;

using Interfaces.Delegates.Constrain;
using Interfaces.Delegates.Format;
using Interfaces.Models.Dependencies;

using Interfaces.Controllers.Logs;

using Attributes;

using Models.Units;

namespace Delegates.Constrain
{
    public class ConstrainExecutionAsyncDelegate : IConstrainAsyncDelegate<int>
    {
        readonly IActionLogController actionLogController;
        readonly IFormatDelegate<long, string> formatSecondsDelegate;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Logs.ActionLogController,Controllers",
            "Delegates.Format.Numbers.FormatSecondsDelegate,Delegates")]
        public ConstrainExecutionAsyncDelegate(
            IActionLogController actionLogController,
            IFormatDelegate<long, string> formatSecondsDelegate)
        {
            this.actionLogController = actionLogController;
            this.formatSecondsDelegate = formatSecondsDelegate;
        }

        public async Task ConstrainAsync(int delaySeconds)
        {
            actionLogController.StartAction(
                $"Sleeping {formatSecondsDelegate.Format(delaySeconds)} before next operation");

            for (var ii = 0; ii < delaySeconds; ii++)
            {
                await Task.Delay(1000);
                // await statusController.UpdateProgressAsync(
                //     throttleTask,
                //     ii + 1,
                //     delaySeconds,
                //     "Countdown",
                //     TimeUnits.Seconds);
            }

            actionLogController.CompleteAction();
        }
    }
}
