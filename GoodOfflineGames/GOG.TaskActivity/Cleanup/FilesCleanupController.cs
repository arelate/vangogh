using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Reporting;
using Interfaces.Enumeration;
using Interfaces.Directory;

using Models.Uris;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Cleanup
{
    public class FilesCleanupController: TaskActivityController
    {
        private IDataController<long> scheduledCleanupDataController;
        private IEnumerateDelegate<string> filesEnumerationController;
        private IEnumerateDelegate<string> directoryEnumerationController;
        private IDirectoryController directoryController;

        public FilesCleanupController(
            IDataController<long> scheduledCleanupDataController,
            IEnumerateDelegate<string> filesEnumerationController,
            IEnumerateDelegate<string> directoryEnumerationController,
            IDirectoryController directoryController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.scheduledCleanupDataController = scheduledCleanupDataController;
            this.filesEnumerationController = filesEnumerationController;
            this.directoryEnumerationController = directoryEnumerationController;
            this.directoryController = directoryController;
        }

        public async override Task ProcessTaskAsync()
        {
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
                        taskReportingController.ReportWarning(file);
                    }
                }
            }
        }
    }
}
