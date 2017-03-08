using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Validation;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Expectation;
using Interfaces.Routing;
using Interfaces.TaskStatus;

namespace GOG.TaskActivities.Validation
{
    public class ProcessValidationController : TaskActivityController
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IValidationController validationController;
        private IEnumerateDelegate<string> gameDetailsManualUrlsEnumerationController;
        private IExpectedDelegate<string> validationExpectedDelegate;
        private IDataController<long> updatedDataController;
        private IDataController<long> lastKnownValidDataController;
        private IDataController<long> scheduledCleanupDataController;
        private IRoutingController routingController;

        public ProcessValidationController(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IValidationController validationController,
            IEnumerateDelegate<string> gameDetailsManualUrlsEnumerationController,
            IExpectedDelegate<string> validationExpectedDelegate,
            IDataController<long> updatedDataController,
            IDataController<long> lastKnownValidDataController,
            IDataController<long> scheduledCleanupDataController,
            IRoutingController routingController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
            this.validationController = validationController;
            this.gameDetailsManualUrlsEnumerationController = gameDetailsManualUrlsEnumerationController;
            this.validationExpectedDelegate = validationExpectedDelegate;

            this.updatedDataController = updatedDataController;
            this.lastKnownValidDataController = lastKnownValidDataController;
            this.scheduledCleanupDataController = scheduledCleanupDataController;
            this.routingController = routingController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var validateProductFilesTask = taskStatusController.Create(taskStatus, "Validate updated products files");

            var counter = 0;

            var updatedProducts = updatedDataController.EnumerateIds().ToArray();

            foreach (var id in updatedProducts)
            {
                var productIsValid = true;

                var manualUrls = await gameDetailsManualUrlsEnumerationController.EnumerateAsync(id);

                taskStatusController.UpdateProgress(validateProductFilesTask,
                    ++counter,
                    updatedProducts.Count(),
                    id.ToString());

                foreach (var manualUrl in manualUrls)
                {
                    var resolvedUri = await routingController.TraceRouteAsync(id, manualUrl);

                    if (!validationExpectedDelegate.Expected(resolvedUri))
                        continue;

                    // use directory from source and file from resolved URI
                    var localFile = Path.Combine(
                        getDirectoryDelegate.GetDirectory(manualUrl),
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
                    await lastKnownValidDataController.UpdateAsync(removeUpdateTask, id);
                    await updatedDataController.RemoveAsync(removeUpdateTask, id);
                    await scheduledCleanupDataController.UpdateAsync(removeUpdateTask, id);
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
