using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Reporting;
using Interfaces.Download;
using Interfaces.Data;
using Interfaces.Destination;
using Interfaces.Network;

using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Processing
{
    public class ProcessScheduledDownloadsController : TaskActivityController
    {
        private ProductDownloadTypes[] downloadTypesFilter;
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<ProductRoutes> productRoutesDataController;
        private INetworkController networkController;
        private IDownloadController downloadController;
        private IDestinationController destinationController;

        public ProcessScheduledDownloadsController(
            ProductDownloadTypes[] downloadTypesFilter,
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<ProductRoutes> productRoutesDataController,
            INetworkController networkController,
            IDownloadController downloadController,
            IDestinationController destinationController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.downloadTypesFilter = downloadTypesFilter;
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.productRoutesDataController = productRoutesDataController;
            this.networkController = networkController;
            this.downloadController = downloadController;
            this.destinationController = destinationController;
        }

        public override async Task ProcessTaskAsync()
        {
            var counter = 0;
            var total = updatedDataController.Count();

            taskReportingController.StartTask("Process updated downloads");
            foreach (var id in updatedDataController.EnumerateIds())
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                var productRoutes = await productRoutesDataController.GetByIdAsync(id);
                if (productRoutes == null)
                {
                    productRoutes = new ProductRoutes()
                    {
                        Id = productDownloads.Id,
                        Title = productDownloads.Title,
                        Routes = new List<ProductRoutesEntry>()
                    };
                }

                taskReportingController.StartTask(
                        "Process downloads for product {0}/{1}: {2}",
                        ++counter,
                        total,
                        productDownloads.Title);

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.ToArray();

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    if (!downloadTypesFilter.Contains(entry.Type)) continue;

                    taskReportingController.StartTask(
                        "Download entry {0}/{1}: {2}",
                        ii + 1,
                        downloadEntries.Length,
                        Enum.GetName(typeof(ProductDownloadTypes), entry.Type));

                    var resolvedUri = string.Empty;

                    try
                    {
                        var response = await networkController.GetResponse(HttpMethod.Get, entry.SourceUri);

                        resolvedUri = response.RequestMessage.RequestUri.ToString();

                        if (entry.Type == ProductDownloadTypes.ProductFile)
                        {
                            var existingRouteUpdated = false;
                            foreach (var route in productRoutes.Routes)
                                if (route.Source == entry.SourceUri)
                                {
                                    route.Destination = resolvedUri;
                                    existingRouteUpdated = true;
                                    break;
                                }

                            if (!existingRouteUpdated)
                                productRoutes.Routes.Add(new ProductRoutesEntry()
                                {
                                    Source = entry.SourceUri,
                                    Destination = resolvedUri
                                });

                            await productRoutesDataController.UpdateAsync(productRoutes);
                        }

                        await downloadController.DownloadFileAsync(response, entry.Destination);
                    }
                    catch (Exception ex)
                    {
                        taskReportingController.ReportWarning(ex.Message);
                    }

                    taskReportingController.CompleteTask();

                    //taskReportingController.StartTask("Remove successfully downloaded scheduled entry");

                    //productDownloads.Downloads.Remove(entry);
                    //await productDownloadsDataController.UpdateAsync(productDownloads);

                    //taskReportingController.CompleteTask();
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
