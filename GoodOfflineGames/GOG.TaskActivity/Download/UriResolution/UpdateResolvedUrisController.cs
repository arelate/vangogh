using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Data;
using Interfaces.UriResolution;
using Interfaces.Throttle;

using GOG.TaskActivities.Abstract;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Download.UriResolution
{
    public class UpdateResolvedUrisController: TaskActivityController
    {
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IUriResolutionController uriResolutionController;
        private IThrottleController throttleController;

        public UpdateResolvedUrisController(
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IUriResolutionController uriResolutionController,
            IThrottleController throttleController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.uriResolutionController = uriResolutionController;
            this.throttleController = throttleController;
        }

        public async override Task ProcessTask()
        {
            taskReportingController.StartTask("Resolve product files Uris");

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var productDownloads = await productDownloadsDataController.GetById(id);

                taskReportingController.StartTask("Resolve Uris for product: {0}", productDownloads.Title);

                foreach (var download in productDownloads.Downloads)
                {
                    if (download.Type != ProductDownloadTypes.ProductFile) continue;

                    download.ResolvedUri = await uriResolutionController.ResolveUri(download.SourceUri);
                }

                taskReportingController.CompleteTask();

                taskReportingController.StartTask("Update downloads for product: {0}", productDownloads.Title);
                await productDownloadsDataController.Update(productDownloads);
                taskReportingController.CompleteTask();

                throttleController?.Throttle();
            }

            taskReportingController.CompleteTask();
        }
    }
}
