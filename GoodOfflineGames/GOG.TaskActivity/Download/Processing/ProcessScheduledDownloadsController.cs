using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Interfaces.Download;
using Interfaces.Data;
using Interfaces.Destination;
using Interfaces.Network;
using Interfaces.Routing;
using Interfaces.Eligibility;
using Interfaces.TaskStatus;

using Models.ProductDownloads;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Processing
{
    public class ProcessScheduledDownloadsController : TaskActivityController
    {
        private ProductDownloadTypes downloadType;
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IRoutingController routingController;
        private INetworkController networkController;
        private IDownloadController downloadController;
        private IDestinationController destinationController;
        private IEligibilityDelegate<ProductDownloadEntry> updateRouteEligibilityDelegate;
        private IEligibilityDelegate<ProductDownloadEntry> removeEntryEligibilityDelegate;

        public ProcessScheduledDownloadsController(
            ProductDownloadTypes downloadType,
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IRoutingController routingController,
            INetworkController networkController,
            IDownloadController downloadController,
            IDestinationController destinationController,
            IEligibilityDelegate<ProductDownloadEntry> updateRouteEligibilityDelegate,
            IEligibilityDelegate<ProductDownloadEntry> removeEntryEligibilityDelegate,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.downloadType = downloadType;
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.routingController = routingController;
            this.networkController = networkController;
            this.downloadController = downloadController;
            this.destinationController = destinationController;

            this.updateRouteEligibilityDelegate = updateRouteEligibilityDelegate;
            this.removeEntryEligibilityDelegate = removeEntryEligibilityDelegate;
        }

        public override async Task ProcessTaskAsync()
        {
            var counter = 0;
            var updated = updatedDataController.EnumerateIds().ToArray();
            var total = updated.Length;

            var processAllDownloadsTask = taskStatusController.Create(taskStatus, "Process updated downloads");

            var processProductDownloadsTask = taskStatusController.Create(processAllDownloadsTask, "Process downloads for product");

            foreach (var id in updated)
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                taskStatusController.UpdateProgress(
                    processProductDownloadsTask,
                    ++counter,
                    total,
                    productDownloads.Title);

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.FindAll(d => d.Type == downloadType).ToArray();

                var processDownloadEntriesTask = taskStatusController.Create(processProductDownloadsTask, "Download product entries");

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    taskStatusController.UpdateProgress(
                        processDownloadEntriesTask,
                        ii + 1,
                        downloadEntries.Length,
                        entry.SourceUri);

                    var resolvedUri = string.Empty;

                    var downloadEntryTask = taskStatusController.Create(processDownloadEntriesTask, "Download entry");

                    try
                    {
                        using (var response = await networkController.GetResponse(HttpMethod.Get, entry.SourceUri))
                        {
                            resolvedUri = response.RequestMessage.RequestUri.ToString();

                            if (updateRouteEligibilityDelegate.IsEligible(entry))
                                await routingController.UpdateRouteAsync(
                                    productDownloads.Id,
                                    productDownloads.Title,
                                    entry.SourceUri,
                                    resolvedUri);

                            await downloadController.DownloadFileAsync(response, entry.Destination, downloadEntryTask);
                        }
                    }
                    catch (Exception ex)
                    {
                        taskStatusController.Warn(processDownloadEntriesTask, ex.Message);
                    }

                    taskStatusController.Complete(downloadEntryTask);

                    // there is no value in trying to redownload images/screenshots - so remove them on success
                    // we won't be removing anything else as it might be used in the later steps
                    if (removeEntryEligibilityDelegate.IsEligible(entry))
                    {
                        var removeEntryTask = taskStatusController.Create(processDownloadEntriesTask, "Remove successfully downloaded scheduled entry");

                        productDownloads.Downloads.Remove(entry);
                        await productDownloadsDataController.UpdateAsync(productDownloads);

                        taskStatusController.Complete(removeEntryTask);
                    }

                    taskStatusController.Complete(processDownloadEntriesTask);
                }
            }

            taskStatusController.Complete(processProductDownloadsTask);

            taskStatusController.Complete(processAllDownloadsTask);
        }
    }
}
