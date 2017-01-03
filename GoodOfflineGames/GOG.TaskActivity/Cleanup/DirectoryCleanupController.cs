using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Directory;
using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Destination;
using Interfaces.RecycleBin;
using Interfaces.Reporting;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Cleanup
{
    public class DirectoryCleanupController: TaskActivityController
    {
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<string> directoryEnumerationController;
        private IDestinationController destinationController;
        private IRecycleBinController recycleBinController;
        private IDirectoryController directoryController;

        public DirectoryCleanupController(
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<string> directoryEnumerationController,
            IDestinationController destinationController,
            IDirectoryController directoryController,
            IRecycleBinController recycleBinController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.directoryEnumerationController = directoryEnumerationController;
            this.destinationController = destinationController;
            this.recycleBinController = recycleBinController;
            this.directoryController = directoryController;
        }

        public override async Task ProcessTaskAsync()
        {
            taskReportingController.StartTask("Enumerate expected product files directories");

            var gameDetailsIds = gameDetailsDataController.EnumerateIds();
            var expectedDirectories = new List<string>(gameDetailsIds.Count());

            foreach (var id in gameDetailsIds)
                expectedDirectories.AddRange(await directoryEnumerationController.EnumerateAsync(id));

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Enumerate actual product files directories");

            var rootDirectory = destinationController.GetDirectory(string.Empty);
            var actualDirectories = directoryController.EnumerateDirectories(rootDirectory);

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Enumerate unexpected product files directories");

            var unexpectedDirectories = new List<string>();

            foreach (var directory in actualDirectories)
                if (!expectedDirectories.Contains(directory))
                    unexpectedDirectories.Add(directory);

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Cleaning up unexpected directories");

            foreach (var directory in unexpectedDirectories)
            {
                taskReportingController.StartTask(
                    string.Format(
                        "Moving {0} to recycle bin",
                        directory));
                recycleBinController.MoveDirectoryToRecycleBin(directory);
                taskReportingController.CompleteTask();
            }

            taskReportingController.CompleteTask();
        }
    }
}
