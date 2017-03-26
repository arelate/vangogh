using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductScreenshots;

namespace GOG.Controllers.DownloadSources
{
    public class ScreenshotsDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> updatedDataController;
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IImageUriController screenshotUriController;
        private ITaskStatusController taskStatusController;

        public ScreenshotsDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<ProductScreenshots> screenshotsDataController,
            IImageUriController screenshotUriController,

            ITaskStatusController taskStatusController)
        {
            this.updatedDataController = updatedDataController;
            this.screenshotsDataController = screenshotsDataController;
            this.screenshotUriController = screenshotUriController;
            this.taskStatusController = taskStatusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(ITaskStatus taskStatus)
        {
            var processUpdatesTask = taskStatusController.Create(taskStatus, "Process screenshot updates");

            var screenshotsSources = new Dictionary<long, IList<string>>();
            var current = 0;
            var total = updatedDataController.Count();

            var processUpdatedProductsScreenshotsTask = taskStatusController.Create(processUpdatesTask, "Process updated product screenshots");

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var productScreenshots = await screenshotsDataController.GetByIdAsync(id);

                if (productScreenshots == null)
                {
                    taskStatusController.Warn(processUpdatedProductsScreenshotsTask, $"Product {id} doesn't have screenshots");
                    continue;
                }

                taskStatusController.UpdateProgress(
                    processUpdatesTask, 
                    ++current, 
                    total,
                    productScreenshots.Title);

                screenshotsSources.Add(id, new List<string>());

                foreach (var uri in productScreenshots.Uris)
                    screenshotsSources[id].Add(screenshotUriController.ExpandUri(uri));
            }

            taskStatusController.Complete(processUpdatedProductsScreenshotsTask);
            taskStatusController.Complete(processUpdatesTask);

            return screenshotsSources;
        }
    }
}
