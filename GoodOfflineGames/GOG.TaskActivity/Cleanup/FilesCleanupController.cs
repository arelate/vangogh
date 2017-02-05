using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Directory;
using Interfaces.Eligibility;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.RecycleBin;
using Interfaces.TaskStatus;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Cleanup
{
    public class FilesCleanupController : TaskActivityController
    {
        private IDataController<long> scheduledCleanupDataController;
        private IDataController<AccountProduct> accountProductsDataController;
        private IEnumerateDelegate<string> filesEnumerationController;
        private IEnumerateDelegate<string> directoryEnumerationController;
        private IDirectoryController directoryController;
        private IEligibilityDelegate<string> fileValidationEligibilityController;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IRecycleBinController recycleBinController;

        public FilesCleanupController(
            IDataController<long> scheduledCleanupDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IEnumerateDelegate<string> filesEnumerationController,
            IEnumerateDelegate<string> directoryEnumerationController,
            IDirectoryController directoryController,
            IEligibilityDelegate<string> fileValidationEligibilityController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IRecycleBinController recycleBinController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.scheduledCleanupDataController = scheduledCleanupDataController;
            this.accountProductsDataController = accountProductsDataController;
            this.filesEnumerationController = filesEnumerationController;
            this.directoryEnumerationController = directoryEnumerationController;
            this.directoryController = directoryController;
            this.fileValidationEligibilityController = fileValidationEligibilityController;
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

        private async Task<IList<string>> GetUnexpectedFiles(long id)
        {
            var productDirectories = await directoryEnumerationController.EnumerateAsync(id);

            var expectedFiles = await filesEnumerationController.EnumerateAsync(id);

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

                recycleBinController.MoveFileToRecycleBin(file);

                if (fileValidationEligibilityController.IsEligible(file))
                {
                    var validationFile = Path.Combine(
                        getDirectoryDelegate.GetDirectory(),
                        getFilenameDelegate.GetFilename(file));

                    var deleteValidationFileTask = taskStatusController.Create(
                        cleanupProductFilesTask,
                        string.Format(
                            "Move validation file to recycle bin: {0}",
                            validationFile));
                    recycleBinController.MoveFileToRecycleBin(validationFile);
                    taskStatusController.Complete(deleteValidationFileTask);
                }
            }

            await RemoveScheduledCleanupEntry(id, cleanupProductFilesTask);
            taskStatusController.Complete(cleanupProductFilesTask);
        }

        public async override Task ProcessTaskAsync()
        {
            var cleanupAllFilesTask = taskStatusController.Create(taskStatus, "Clean up older versions of the products files");

            var cleanupProductTask = taskStatusController.Create(cleanupAllFilesTask, "Clean up product files");
            var counter = 0;

            var scheduledCleanupIds = scheduledCleanupDataController.EnumerateIds();

            foreach (var id in scheduledCleanupIds)
            {
                var accountProduct = await accountProductsDataController.GetByIdAsync(id);
                if (accountProduct == null)
                {
                    taskStatusController.Warn(
                        cleanupProductTask,
                        string.Format(
                            "Account product doesn't exist: {0}",
                            id));
                    continue;
                }

                taskStatusController.UpdateProgress(
                    cleanupProductTask,
                    counter++,
                    scheduledCleanupIds.Count(),
                    accountProduct.Title);

                var unexpectedFiles = await GetUnexpectedFiles(id);

                if (unexpectedFiles.Count == 0)
                {
                    await RemoveScheduledCleanupEntry(id, cleanupAllFilesTask);
                    continue;
                };

                await MoveUnexpectedFilesToRecycleBin(id, unexpectedFiles, cleanupAllFilesTask);
            }

            taskStatusController.Complete(cleanupAllFilesTask);
        }
    }
}
