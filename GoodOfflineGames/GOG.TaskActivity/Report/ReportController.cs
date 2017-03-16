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

namespace GOG.TaskActivities.LogTaskStatus
{
    public class ReportController: TaskActivityController
    {
        private IViewController taskStatusViewController;

        public ReportController(
            IViewController taskStatusViewController,
            ITaskStatusController taskStatusController):
            base(taskStatusController)
        {
            this.taskStatusViewController = taskStatusViewController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var reportTask = taskStatusController.Create(taskStatus, "Presenting report on application task status");
            await taskStatusViewController.PresentViewsAsync();
            taskStatusController.Complete(reportTask);
        }

    }
}
