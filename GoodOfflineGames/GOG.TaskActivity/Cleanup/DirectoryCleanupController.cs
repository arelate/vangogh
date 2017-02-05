using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Directory;
using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Destination.Directory;
using Interfaces.RecycleBin;
using Interfaces.TaskStatus;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Cleanup
{
    public class DirectoryCleanupController: TaskActivityController
    {
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<string> directoryEnumerationController;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IRecycleBinController recycleBinController;
        private IDirectoryController directoryController;

        public DirectoryCleanupController(
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<string> directoryEnumerationController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IDirectoryController directoryController,
            IRecycleBinController recycleBinController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController):
            base(
                taskStatus,
                taskStatusController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.directoryEnumerationController = directoryEnumerationController;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.recycleBinController = recycleBinController;
            this.directoryController = directoryController;
        }

        public override async Task ProcessTaskAsync()
        {
            var cleanupDirectoriesTask = taskStatusController.Create(taskStatus, "Cleanup product directories");

            var enumerateExpectedDirectoriesTask = taskStatusController.Create(cleanupDirectoriesTask, "Enumerate expected product files directories");

            var gameDetailsIds = gameDetailsDataController.EnumerateIds();
            var gameDetailsIdsCount = gameDetailsIds.Count();
            var expectedDirectories = new List<string>(gameDetailsIdsCount);
            var counter = 0;

            foreach (var id in gameDetailsIds)
            {
                taskStatusController.UpdateProgress(enumerateExpectedDirectoriesTask, ++counter, gameDetailsIdsCount, id.ToString());
                expectedDirectories.AddRange(await directoryEnumerationController.EnumerateAsync(id));
            }

            taskStatusController.Complete(enumerateExpectedDirectoriesTask);

            var enumerateActualDirectoriesTask = taskStatusController.Create(cleanupDirectoriesTask, "Enumerate actual product files directories");

            var rootDirectory = getDirectoryDelegate.GetDirectory();
            var actualDirectories = directoryController.EnumerateDirectories(rootDirectory);

            taskStatusController.Complete(enumerateActualDirectoriesTask);

            var enumerateUnexpectedDirectoriesTask = taskStatusController.Create(cleanupDirectoriesTask, "Enumerate unexpected product files directories");

            var unexpectedDirectories = new List<string>();

            foreach (var directory in actualDirectories)
                if (!expectedDirectories.Contains(directory))
                    unexpectedDirectories.Add(directory);

            taskStatusController.Complete(enumerateUnexpectedDirectoriesTask);

            var cleanupUnexpectedDirectoriesTask = taskStatusController.Create(cleanupDirectoriesTask, "Clean up unexpected directories");
            counter = 0;

            foreach (var directory in unexpectedDirectories)
            {
                taskStatusController.UpdateProgress(
                    cleanupUnexpectedDirectoriesTask,
                    counter++,
                    unexpectedDirectories.Count,
                    directory);
                recycleBinController.MoveDirectoryToRecycleBin(directory);
            }

            taskStatusController.Complete(cleanupUnexpectedDirectoriesTask);

            taskStatusController.Complete(cleanupDirectoriesTask);
        }
    }
}
