using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.ViewUpdates;

namespace GOG.Activities.Report
{
    public class ReportActivity: Activity
    {
        private IPostViewUpdateAsyncDelegate postViewUpdateAsyncDelegate;

        public ReportActivity(
            IPostViewUpdateAsyncDelegate postViewUpdateAsyncDelegate,
            IStatusController statusController):
            base(statusController)
        {
            this.postViewUpdateAsyncDelegate = postViewUpdateAsyncDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var reportTask = statusController.Create(status, "Presenting report on application task status");
            await postViewUpdateAsyncDelegate.PostViewUpdateAsync();
            statusController.Complete(reportTask);
        }

    }
}
