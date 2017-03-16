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
        //private IGetFilenameDelegate logsFilenameDelegate;
        //private IGetDirectoryDelegate logsDirectoryDelegate;

        private IViewController taskStatusViewController;

        //private ISerializedStorageController serializedStorageController;

        public ReportController(
            IViewController taskStatusViewController,
            //IGetDirectoryDelegate logsDirectoryDelegate,
            //IGetFilenameDelegate logsFilenameDelegate,
            //ISerializedStorageController serializedStorageController,
            ITaskStatusController taskStatusController):
            base(taskStatusController)
        {
            this.taskStatusViewController = taskStatusViewController;

            //this.logsFilenameDelegate = logsFilenameDelegate;
            //this.logsDirectoryDelegate = logsDirectoryDelegate;
            //this.serializedStorageController = serializedStorageController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            //var uri = System.IO.Path.Combine(
            //    logsDirectoryDelegate.GetDirectory(),
            //    logsFilenameDelegate.GetFilename());

            //foreach (var taskStatus in taskStatusTreeToListController.ToList(taskStatus))
            //{

            //}

            //var saveLogTask = taskStatusController.Create(taskStatus, "Save task status report");
            //await serializedStorageController.SerializePushAsync(uri, taskStatusList);
            //taskStatusController.Complete(saveLogTask);
        }

    }
}
