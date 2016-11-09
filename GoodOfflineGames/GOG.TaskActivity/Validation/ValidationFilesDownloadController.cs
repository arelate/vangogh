using System.Threading.Tasks;

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
        private IValidationDataDownloadController validationDataDownloadController;

        public ValidationFilesDownloadController(
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IValidationDataDownloadController validationDataDownloadController,
            ITaskReportingController taskReportingController): 
            base(taskReportingController)
        {
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.validationDataDownloadController = validationDataDownloadController;
        }

        public override async Task ProcessTask()
        {
            foreach (var id in updatedDataController.EnumerateIds())
            {
                var productDownloads = await productDownloadsDataController.GetById(id);
                if (productDownloads == null) continue;

                foreach (var download in productDownloads.Downloads)
                {
                    if (download.Type != ProductDownloadTypes.ProductFile) continue;
                    if (string.IsNullOrEmpty(download.ResolvedUri)) continue;

                    await validationDataDownloadController.DownloadValidationData(download.ResolvedUri);
                }
            }
        }

    }
}
