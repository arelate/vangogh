using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Reporting;
using Interfaces.Validation;
using Interfaces.Destination;
using Interfaces.Data;
using Interfaces.Routing;
using Interfaces.Eligibility;

using Models.ProductDownloads;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Validation
{
    public class ProcessValidationController : TaskActivityController
    {
        private IDestinationController destinationController;
        private IValidationController validationController;
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IRoutingController routingController;
        private IEligibilityDelegate<ProductDownloadEntry> validationEligibilityDelegate;
        private IDataController<long> lastKnownValidDataController;

        public ProcessValidationController(
            IDestinationController destinationController,
            IValidationController validationController,
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IRoutingController routingController,
            IEligibilityDelegate<ProductDownloadEntry> validationEligibilityDelegate,
            IDataController<long> lastKnownValidDataController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.destinationController = destinationController;
            this.validationController = validationController;

            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.routingController = routingController;
            this.validationEligibilityDelegate = validationEligibilityDelegate;
            this.lastKnownValidDataController = lastKnownValidDataController;
        }

        public override async Task ProcessTaskAsync()
        {
            taskReportingController.StartTask("Validate product files");

            var counter = 0;

            var updatedProducts = updatedDataController.EnumerateIds().ToArray();

            foreach (var id in updatedProducts)
            {
                var productIsValid = true;

                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                taskReportingController.StartTask("Validate product {0}/{1}: {2}",
                    ++counter,
                    updatedProducts.Count(),
                    productDownloads.Title);

                foreach (var download in productDownloads.Downloads)
                {
                    if ((bool) !validationEligibilityDelegate?.IsEligible(download))
                        continue;

                    var resolvedUri = await routingController.TraceRouteAsync(id, download.SourceUri);

                    // use directory from source and file from resolved URI
                    var localFile = Path.Combine(
                        destinationController.GetDirectory(download.SourceUri),
                        destinationController.GetFilename(resolvedUri));

                    try
                    {
                        taskReportingController.StartTask("Validate product file: {0}", localFile);
                        await validationController.ValidateAsync(localFile);
                        productIsValid &= true;
                        taskReportingController.CompleteTask();
                    }
                    catch (Exception ex)
                    {
                        taskReportingController.ReportFailure(ex.Message);
                        productIsValid &= false;
                    }
                }

                taskReportingController.CompleteTask();

                if (productIsValid)
                {
                    taskReportingController.StartTask("Congratulations, all product files are valid! removing product from updates: {0}", productDownloads.Title);
                    await lastKnownValidDataController.UpdateAsync(id);
                    await updatedDataController.RemoveAsync(id);
                    taskReportingController.CompleteTask();
                }
                else
                {
                    taskReportingController.StartTask("Unfortunately, some product files failed validation, updating last known valid state: {0}", productDownloads.Title);
                    await lastKnownValidDataController.RemoveAsync(id);
                    taskReportingController.CompleteTask();
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
