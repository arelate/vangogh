using System;
using System.Threading.Tasks;
using System.IO;

using Interfaces.TaskStatus;
using Interfaces.Destination;
using Interfaces.SerializedStorage;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Logging
{
    public class LogController : TaskActivityController
    {
        private IDestinationController logDestinationController;
        private ISerializedStorageController serializedStorageController;

        public LogController(
            IDestinationController logDestinationController,
            ISerializedStorageController serializedStorageController,
            ITaskStatus taskStatus, 
            ITaskStatusController taskStatusController) : 
            base(taskStatus, taskStatusController)
        {
            this.logDestinationController = logDestinationController;
            this.serializedStorageController = serializedStorageController;
        }

        public override async Task ProcessTaskAsync()
        {
            var serializeLogTask = taskStatusController.Create(taskStatus, "Save operations log");

            var uri = Path.Combine(
                logDestinationController.GetDirectory(string.Empty),
                logDestinationController.GetFilename(DateTime.UtcNow.ToFileTimeUtc().ToString()));

            await serializedStorageController.SerializePushAsync(uri, taskStatus);

            taskStatusController.Complete(serializeLogTask);
        }
    }
}
