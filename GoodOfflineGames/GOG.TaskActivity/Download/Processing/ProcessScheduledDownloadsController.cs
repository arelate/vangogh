using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

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
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<ProductRoutes> productRoutesDataController;
        private IDataController<ScheduledValidation> scheduledValidationsDataController;
        private INetworkController networkController;
        private IDownloadController downloadController;
        private IDestinationController destinationController;

        public ProcessScheduledDownloadsController(
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<ProductRoutes> productRoutesDataController,
            IDataController<ScheduledValidation> scheduledValidationsDataController,
            INetworkController networkController,
            IDownloadController downloadController,
            IDestinationController destinationController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.productRoutesDataController = productRoutesDataController;
            this.scheduledValidationsDataController = scheduledValidationsDataController;
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

                    // schedule validation for downloaded file
                    if (entry.Type == ProductDownloadTypes.ProductFile)
                    {
                        taskReportingController.StartTask("Schedule validation for downloaded file");

                        var scheduledValidation = await scheduledValidationsDataController.GetByIdAsync(id);
                        if (scheduledValidation == null)
                        {
                            scheduledValidation = new ScheduledValidation()
                            {
                                Id = productDownloads.Id,
                                Title = productDownloads.Title,
                                Files = new List<string>()
                            };
                        }

                        var filePath = Path.Combine(entry.Destination,
                            destinationController.GetFilename(resolvedUri));

                        if (!scheduledValidation.Files.Contains(filePath))
                        {
                            scheduledValidation.Files.Add(filePath);
                            await scheduledValidationsDataController.UpdateAsync(scheduledValidation);
                        }

                        taskReportingController.CompleteTask();
                    }

                    taskReportingController.StartTask("Remove successfully downloaded scheduled entry");

                    productDownloads.Downloads.Remove(entry);
                    await productDownloadsDataController.UpdateAsync(productDownloads);

                    taskReportingController.CompleteTask();
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
