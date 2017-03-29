using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.TaskStatus;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.SerializedStorage;
using Interfaces.ViewController;

namespace GOG.Activities.LogTaskStatus
{
    public class ReportActivity: Activity
    {
        private IViewController taskStatusViewController;

        public ReportActivity(
            IViewController taskStatusViewController,
            ITaskStatusController taskStatusController):
            base(taskStatusController)
        {
            this.taskStatusViewController = taskStatusViewController;
        }

        public override async Task ProcessActivityAsync(ITaskStatus taskStatus)
        {
            var reportTask = taskStatusController.Create(taskStatus, "Presenting report on application task status");
            await taskStatusViewController.PresentViewsAsync();
            taskStatusController.Complete(reportTask);
        }

    }
}
