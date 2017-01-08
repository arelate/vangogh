using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Validation;
using Interfaces.Destination;
using Interfaces.Data;
using Interfaces.Routing;
using Interfaces.Eligibility;
using Interfaces.TaskStatus;

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
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
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
            var validateAllTask = taskStatusController.Create(taskStatus, "Validate all updated products files");

            var counter = 0;

            var updatedProducts = updatedDataController.EnumerateIds().ToArray();

            var validateProductTask = taskStatusController.Create(validateAllTask, "Validate updated product");

            foreach (var id in updatedProducts)
            {
                var productIsValid = true;

                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                taskStatusController.UpdateProgress(validateProductTask,
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
                        var validateFileTask = taskStatusController.Create(
                            validateProductTask, 
                            string.Format(
                                "Validate product file: {0}", 
                                localFile));
                        await validationController.ValidateAsync(localFile, validateFileTask);
                        productIsValid &= true;
                        taskStatusController.Complete(validateFileTask);
                    }
                    catch (Exception ex)
                    {
                        taskStatusController.Fail(validateProductTask, ex.Message);
                        productIsValid &= false;
                    }
                }

                if (productIsValid)
                {
                    var removeUpdateTask = taskStatusController.Create(validateProductTask, "All product files are valid. Remove product from updates and scheduling cleanup");
                    await lastKnownValidDataController.UpdateAsync(id);
                    await updatedDataController.RemoveAsync(id);
                    await scheduledCleanupDataController.UpdateAsync(id);
                    taskStatusController.Complete(removeUpdateTask);
                }
            }

            taskStatusController.Complete(validateProductTask);
            taskStatusController.Complete(validateAllTask);
        }
    }
}
