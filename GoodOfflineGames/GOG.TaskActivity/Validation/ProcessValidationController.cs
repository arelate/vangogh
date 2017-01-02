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
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<long> updatedDataController;
        private IDataController<long> lastKnownValidDataController;
        private IDataController<long> scheduledCleanupDataController;
        private IRoutingController routingController;
        private IEligibilityDelegate<ProductDownloadEntry> validationEligibilityDelegate;

        public ProcessValidationController(
            IDestinationController destinationController,
            IValidationController validationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<long> updatedDataController,
            IDataController<long> lastKnownValidDataController,
            IDataController<long> scheduledCleanupDataController,
            IRoutingController routingController,
            IEligibilityDelegate<ProductDownloadEntry> validationEligibilityDelegate,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.destinationController = destinationController;
            this.validationController = validationController;
            this.productDownloadsDataController = productDownloadsDataController;

            this.updatedDataController = updatedDataController;
            this.lastKnownValidDataController = lastKnownValidDataController;
            this.scheduledCleanupDataController = scheduledCleanupDataController;
            this.routingController = routingController;
            this.validationEligibilityDelegate = validationEligibilityDelegate;
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
                    if (!validationEligibilityDelegate.IsEligible(download))
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
                    taskReportingController.StartTask("Congratulations, all product files are valid! removing product from updates and scheduling cleanup: {0}", productDownloads.Title);
                    await lastKnownValidDataController.UpdateAsync(id);
                    await updatedDataController.RemoveAsync(id);
                    await scheduledCleanupDataController.UpdateAsync(id);
                    taskReportingController.CompleteTask();
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
