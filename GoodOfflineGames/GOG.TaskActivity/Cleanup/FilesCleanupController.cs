using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Directory;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.RecycleBin;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.TaskActivities.Cleanup
{
    public class FilesCleanupController : TaskActivityController
    {
        private IDataController<long> scheduledCleanupDataController;
        private IDataController<AccountProduct> accountProductsDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> filesEnumerationController;
        private IEnumerateDelegate<GameDetails> directoryEnumerationController;
        private IDirectoryController directoryController;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IRecycleBinController recycleBinController;

        public FilesCleanupController(
            IDataController<long> scheduledCleanupDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateAsyncDelegate<GameDetails> filesEnumerationController,
            IEnumerateDelegate<GameDetails> directoryEnumerationController,
            IDirectoryController directoryController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IRecycleBinController recycleBinController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.scheduledCleanupDataController = scheduledCleanupDataController;
            this.accountProductsDataController = accountProductsDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.filesEnumerationController = filesEnumerationController;
            this.directoryEnumerationController = directoryEnumerationController;
            this.directoryController = directoryController;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
            this.recycleBinController = recycleBinController;
        }

        private async Task RemoveScheduledCleanupEntry(long id, ITaskStatus taskStatus)
        {
            var removeScheduledCleanupEntry = taskStatusController.Create(taskStatus, "Remove scheduled cleanup entry");
            await scheduledCleanupDataController.RemoveAsync(taskStatus, id);
            taskStatusController.Complete(removeScheduledCleanupEntry);
        }

        private async Task<IList<string>> GetUnexpectedFiles(long id, ITaskStatus taskStatus)
        {
            var gameDetails = await gameDetailsDataController.GetByIdAsync(id);
            var productDirectories = directoryEnumerationController.Enumerate(gameDetails);
            IList<string> expectedFiles = new List<string>();

            try
            {
                expectedFiles = await filesEnumerationController.EnumerateAsync(gameDetails);
            }
            catch (System.ArgumentException ex)
            {
                taskStatusController.Fail(taskStatus, $"Failed to get unexpected files for product {id}, message: {ex.Message}");
                return expectedFiles;
            }

            var actualFiles = new List<string>();
            foreach (var directory in productDirectories)
                actualFiles.AddRange(directoryController.EnumerateFiles(directory));

            var unexpectedFiles = new List<string>();

            foreach (var file in actualFiles)
                if (!expectedFiles.Contains(file))
                    unexpectedFiles.Add(file);

            return unexpectedFiles;
        }

        private async Task MoveUnexpectedFilesToRecycleBin(long id, IList<string> unexpectedFiles, ITaskStatus taskStatus)
        {
            var cleanupProductFilesTask = taskStatusController.Create(taskStatus, "Move product files to recycle bin");

            var productFilesCounter = 0;

            foreach (var file in unexpectedFiles)
            {
                taskStatusController.UpdateProgress(
                    cleanupProductFilesTask,
                    ++productFilesCounter,
                    unexpectedFiles.Count,
                    file);

                recycleBinController.MoveToRecycleBin(file);

                var validationFile = Path.Combine(
                    getDirectoryDelegate.GetDirectory(),
                    getFilenameDelegate.GetFilename(file));

                var deleteValidationFileTask = taskStatusController.Create(
                    cleanupProductFilesTask,
                    $"Move validation file to recycle bin: {validationFile}");
                recycleBinController.MoveToRecycleBin(validationFile);
                taskStatusController.Complete(deleteValidationFileTask);
            }

            await RemoveScheduledCleanupEntry(id, cleanupProductFilesTask);
            taskStatusController.Complete(cleanupProductFilesTask);
        }

        public async override Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var cleanupAllFilesTask = taskStatusController.Create(taskStatus, "Clean up older versions of the products files");

            var cleanupProductTask = taskStatusController.Create(cleanupAllFilesTask, "Clean up product files");
            var counter = 0;

            var scheduledCleanupIds = scheduledCleanupDataController.EnumerateIds().ToArray();

            foreach (var id in scheduledCleanupIds)
            {
                var accountProduct = await accountProductsDataController.GetByIdAsync(id);
                if (accountProduct == null)
                {
                    taskStatusController.Warn(
                        cleanupProductTask,
                        $"Account product doesn't exist: {id}");
                    continue;
                }

                taskStatusController.UpdateProgress(
                    cleanupProductTask,
                    counter++,
                    scheduledCleanupIds.Count(),
                    accountProduct.Title);

                var unexpectedFiles = await GetUnexpectedFiles(id, cleanupProductTask);

                if (unexpectedFiles.Count == 0)
                {
                    await RemoveScheduledCleanupEntry(id, cleanupAllFilesTask);
                    continue;
                };

                await MoveUnexpectedFilesToRecycleBin(id, unexpectedFiles, cleanupAllFilesTask);
            }

            taskStatusController.Complete(cleanupProductTask);

            taskStatusController.Complete(cleanupAllFilesTask);
        }
    }
}
