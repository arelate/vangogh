using System.Threading.Tasks;
using System.IO;

using Interfaces.Validation;
using Interfaces.Reporting;
using Interfaces.Data;

using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Validation
{
    public class ValidationFilesDownloadController: TaskActivityController
    {
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<ProductRoutes> productRoutesDataController;
        private IValidationDataDownloadController validationDataDownloadController;
        private const string patchPrefix = "patch_";

        public ValidationFilesDownloadController(
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<ProductRoutes> productRoutesDataController,
            IValidationDataDownloadController validationDataDownloadController,
            ITaskReportingController taskReportingController): 
            base(taskReportingController)
        {
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.productRoutesDataController = productRoutesDataController;
            this.validationDataDownloadController = validationDataDownloadController;
        }

        public override async Task ProcessTaskAsync()
        {
            taskReportingController.StartTask("Download validation files");

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                var productRoutes = await productRoutesDataController.GetByIdAsync(id);
                if (productRoutes == null) continue;

                taskReportingController.StartTask("Download validation files for product: {0}", productDownloads.Title);

                foreach (var download in productDownloads.Downloads)
                {
                    if (download.Type != ProductDownloadTypes.ProductFile) continue;
                    //if (string.IsNullOrEmpty(download.ResolvedUri)) continue;

                    var resolvedUri = string.Empty;
                    foreach (var route in productRoutes.Routes)
                    {
                        if (route.Source == download.SourceUri)
                            resolvedUri = route.Destination;
                    }

                    // TODO: Consider something else to do with patches
                    // if (Path.GetFileName(resolvedUri).StartsWith(patchPrefix)) continue

                    if (string.IsNullOrEmpty(resolvedUri)) continue;

                    await validationDataDownloadController.DownloadValidationDataAsync(resolvedUri);
                }

                taskReportingController.CompleteTask();
            }

            taskReportingController.CompleteTask();
        }

    }
}
