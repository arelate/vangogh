using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.ViewController;

namespace GOG.Activities.Report
{
    public class ReportActivity: Activity
    {
        private IViewController<string> statusViewController;

        public ReportActivity(
            IViewController<string> statusViewController,
            IStatusController statusController):
            base(statusController)
        {
            this.statusViewController = statusViewController;
        }

        public override async Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            var reportTask = statusController.Create(status, "Presenting report on application task status");
            await statusViewController.PostUpdateNotificationAsync();
            statusController.Complete(reportTask);
        }

    }
}
