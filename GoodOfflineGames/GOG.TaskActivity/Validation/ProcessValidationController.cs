using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Validation;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
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
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IValidationController validationController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<long> updatedDataController;
        private IDataController<long> lastKnownValidDataController;
        private IDataController<long> scheduledCleanupDataController;
        private IRoutingController routingController;
        private IEligibilityDelegate<ProductDownloadEntry> validationEligibilityDelegate;

        public ProcessValidationController(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
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
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
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
            var validateProductFilesTask = taskStatusController.Create(taskStatus, "Validate updated products files");

            var counter = 0;

            var updatedProducts = updatedDataController.EnumerateIds().ToArray();

            foreach (var id in updatedProducts)
            {
                var productIsValid = true;

                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                taskStatusController.UpdateProgress(validateProductFilesTask,
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
                        getDirectoryDelegate.GetDirectory(download.SourceUri),
                        getFilenameDelegate.GetFilename(resolvedUri));

                    var validateFileTask = taskStatusController.Create(
                        validateProductFilesTask,
                        string.Format(
                            "Validate product file",
                            localFile));

                    try
                    {
                        await validationController.ValidateAsync(localFile, validateFileTask);
                        productIsValid &= true;
                    }
                    catch (Exception ex)
                    {
                        taskStatusController.Fail(validateProductFilesTask, 
                            string.Format(
                                "{0}: {1}", 
                                localFile, 
                                ex.Message));
                        productIsValid &= false;
                    }
                    finally
                    {
                        taskStatusController.Complete(validateFileTask);
                    }
                }

                if (productIsValid)
                {
                    var removeUpdateTask = taskStatusController.Create(
                        validateProductFilesTask,
                        "All product files are valid. Clear product update flag and schedule cleanup");
                    await lastKnownValidDataController.AddAsync(removeUpdateTask, id);
                    await updatedDataController.RemoveAsync(removeUpdateTask, id);
                    await scheduledCleanupDataController.AddAsync(removeUpdateTask, id);
                    taskStatusController.Complete(removeUpdateTask);
                }
                else
                {
                    var removeKnownValidTask = taskStatusController.Create(
                        validateProductFilesTask,
                        "Product files might not be valid. Remove them from known valid");
                    await lastKnownValidDataController.RemoveAsync(removeKnownValidTask, id);
                    taskStatusController.Complete(removeKnownValidTask);
                }
            }

            taskStatusController.Complete(validateProductFilesTask);
        }
    }
}
