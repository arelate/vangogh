using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.ProductTypes;
using Interfaces.Storage;
using Interfaces.Download;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download
{
    public class ProcessScheduledDownloadsController: TaskActivityController
    {
        private IProductTypeStorageController productStorageController;
        private IDownloadController downloadController;

        public ProcessScheduledDownloadsController(
            IProductTypeStorageController productStorageController,
            IDownloadController downloadController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.productStorageController = productStorageController;
            this.downloadController = downloadController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing scheduled downloads");
            var existingScheduledDownloads = await productStorageController.Pull<ScheduledDownload>(ProductTypes.ScheduledDownload);

            var scheduledDownloads = new List<ScheduledDownload>(existingScheduledDownloads);

            taskReportingController.CompleteTask();

            var currentDownload = 0;
            var storagePushNthProduct = 100; // push after updating every nth product

            taskReportingController.StartTask("Process scheduled downloads");
            foreach (var download in scheduledDownloads)
            {
                taskReportingController.StartTask(
                    string.Format(
                        "Download {0}/{1}: {2}", 
                        ++currentDownload, 
                        scheduledDownloads.Count, 
                        download.Description));

                await downloadController.DownloadFile(download.Source, download.Destination);

                existingScheduledDownloads.Remove(download);

                taskReportingController.StartTask("Saving updated scheduled downloads");
                if (currentDownload % storagePushNthProduct == 0)
                    await productStorageController.Push(ProductTypes.ScheduledDownload, existingScheduledDownloads);
                taskReportingController.CompleteTask();

                taskReportingController.CompleteTask();
            }

            taskReportingController.StartTask("Saving updated scheduled downloads");
            await productStorageController.Push(ProductTypes.ScheduledDownload, existingScheduledDownloads);
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();

        }
    }
}
