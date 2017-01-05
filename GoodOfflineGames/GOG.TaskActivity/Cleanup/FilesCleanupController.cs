using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Data;
using Interfaces.Reporting;
using Interfaces.Enumeration;
using Interfaces.Directory;
using Interfaces.Eligibility;
using Interfaces.Destination;
using Interfaces.RecycleBin;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Cleanup
{
    public class FilesCleanupController: TaskActivityController
    {
        private IDataController<long> scheduledCleanupDataController;
        private IEnumerateDelegate<string> filesEnumerationController;
        private IEnumerateDelegate<string> directoryEnumerationController;
        private IDirectoryController directoryController;
        private IEligibilityDelegate<string> fileValidationEligibilityController;
        private IDestinationController validationDestinationController;
        private IRecycleBinController recycleBinController;

        public FilesCleanupController(
            IDataController<long> scheduledCleanupDataController,
            IEnumerateDelegate<string> filesEnumerationController,
            IEnumerateDelegate<string> directoryEnumerationController,
            IDirectoryController directoryController,
            IEligibilityDelegate<string> fileValidationEligibilityController,
            IDestinationController validationDestinationController,
            IRecycleBinController recycleBinController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.scheduledCleanupDataController = scheduledCleanupDataController;
            this.filesEnumerationController = filesEnumerationController;
            this.directoryEnumerationController = directoryEnumerationController;
            this.directoryController = directoryController;
            this.fileValidationEligibilityController = fileValidationEligibilityController;
            this.validationDestinationController = validationDestinationController;
            this.recycleBinController = recycleBinController;
        }

        public async override Task ProcessTaskAsync()
        {
            taskReportingController.StartTask("Cleaning up older versions of the product files");

            foreach (var id in scheduledCleanupDataController.EnumerateIds())
            {
                var productDirectories = await directoryEnumerationController.EnumerateAsync(id);
                var expectedFiles = await filesEnumerationController.EnumerateAsync(id);

                var actualFiles = new List<string>();
                foreach (var directory in productDirectories)
                    actualFiles.AddRange(directoryController.EnumerateFiles(directory));

                foreach (var file in actualFiles)
                {
                    if (!expectedFiles.Contains(file))
                    {
                        taskReportingController.StartTask("Move product file to recycle bin: {0}", file);
                        recycleBinController.MoveFileToRecycleBin(file);
                        taskReportingController.CompleteTask();

                        if (fileValidationEligibilityController.IsEligible(file))
                        {
                            var validationFile = Path.Combine(
                                validationDestinationController.GetDirectory(file),
                                validationDestinationController.GetFilename(file));

                            taskReportingController.StartTask("Move validation file to recycle bin: {0}", validationFile);
                            recycleBinController.MoveFileToRecycleBin(validationFile);
                            taskReportingController.CompleteTask();
                        }

                    }
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
