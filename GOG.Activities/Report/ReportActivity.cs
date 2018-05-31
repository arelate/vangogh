using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.NotifyViewUpdate;

namespace GOG.Activities.Report
{
    public class ReportActivity: Activity
    {
        readonly INotifyViewUpdateOutputContinuousAsyncDelegate notifyViewUpdateOutputContinuousAsyncDelegate;

        public ReportActivity(
            INotifyViewUpdateOutputContinuousAsyncDelegate notifyViewUpdateOutputContinuousAsyncDelegate,
            IStatusController statusController):
            base(statusController)
        {
            this.notifyViewUpdateOutputContinuousAsyncDelegate = notifyViewUpdateOutputContinuousAsyncDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var reportTask = await statusController.CreateAsync(status, "Presenting report on application task status");
            await notifyViewUpdateOutputContinuousAsyncDelegate.NotifyViewUpdateOutputContinuousAsync();
            await statusController.CompleteAsync(reportTask);
        }

    }
}
