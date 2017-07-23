using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.SerializedStorage;
using Interfaces.ViewController;

namespace GOG.Activities.Report
{
    public class ReportActivity: Activity
    {
        private IViewController statusViewController;

        public ReportActivity(
            IViewController statusViewController,
            IStatusController statusController):
            base(statusController)
        {
            this.statusViewController = statusViewController;
        }

        public override async Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            var reportTask = statusController.Create(status, "Presenting report on application task status");
            await statusViewController.PresentViewsAsync();
            statusController.Complete(reportTask);
        }

    }
}
