using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.ProductTypes;
using Interfaces.Storage;
using Interfaces.Download;
using Interfaces.Collection;

using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Processing
{
    public class ProcessScheduledDownloadsController: TaskActivityController
    {
        //private IProductTypeStorageController productStorageController;
        private IDownloadController downloadController;
        private ICollectionController collectionController;

        public ProcessScheduledDownloadsController(
            //IProductTypeStorageController productStorageController,
            IDownloadController downloadController,
            ICollectionController collectionController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            //this.productStorageController = productStorageController;
            this.downloadController = downloadController;
            this.collectionController = collectionController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing scheduled downloads and products");
            var existingScheduledDownloads = new List<ScheduledDownload>(); // await productStorageController.Pull<ScheduledDownload>(ProductTypes.ScheduledDownload);
            var products = new List<Models.Product>(); // await productStorageController.Pull<Models.Product>(ProductTypes.Product);

            var scheduledDownloads = new List<ScheduledDownload>(existingScheduledDownloads);

            taskReportingController.CompleteTask();

            var currentDownload = 0;
            var storagePushNthProduct = 100; // push after updating every nth product

            taskReportingController.StartTask("Process scheduled downloads");
            foreach (var download in scheduledDownloads)
            {
                var product = collectionController.Find(products, p => p.Id == download.Id);

                //taskReportingController.StartTask(
                //        "Download {0}/{1}: {2} for {3}", 
                //        ++currentDownload, 
                //        scheduledDownloads.Count,
                //        System.Enum.GetName(typeof(ScheduledDownloadTypes), download.Type),
                //        product?.Title);

                //await downloadController.DownloadFile(download.Source, download.Destination);

                // trivial file types can safely be removed
                // however files won't be removed here
                // instead they are expected to go through validation
                // and validation task activity would remove validated files
                //if (download.Type != ScheduledDownloadTypes.File)
                //    existingScheduledDownloads.Remove(download);

                if (currentDownload % storagePushNthProduct == 0)
                {
                    taskReportingController.StartTask("Saving updated scheduled downloads");
                    //await productStorageController.Push(ProductTypes.ScheduledDownload, existingScheduledDownloads);
                    taskReportingController.CompleteTask();
                }

                taskReportingController.CompleteTask();
            }

            taskReportingController.StartTask("Saving updated scheduled downloads");
            //await productStorageController.Push(ProductTypes.ScheduledDownload, existingScheduledDownloads);
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();

        }
    }
}
