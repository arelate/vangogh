using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;
using Interfaces.TaskStatus;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.File;

using Models.ProductScreenshots;

namespace GOG.Controllers.DownloadSources
{
    public class ScreenshotsDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IImageUriController screenshotUriController;
        private IGetDirectoryDelegate screenshotsDirectoryDelegate;
        private IFileController fileController;
        private ITaskStatusController taskStatusController;

        public ScreenshotsDownloadSourcesController(
            IDataController<ProductScreenshots> screenshotsDataController,
            IImageUriController screenshotUriController,
            IGetDirectoryDelegate screenshotsDirectoryDelegate,
            IFileController fileController,
            ITaskStatusController taskStatusController)
        {
            this.screenshotsDataController = screenshotsDataController;
            this.screenshotUriController = screenshotUriController;
            this.screenshotsDirectoryDelegate = screenshotsDirectoryDelegate;
            this.fileController = fileController;
            this.taskStatusController = taskStatusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(ITaskStatus taskStatus)
        {
            var processUpdatesTask = taskStatusController.Create(taskStatus, "Process screenshots updates");

            var screenshotsSources = new Dictionary<long, IList<string>>();
            var current = 0;
            var total = screenshotsDataController.Count();

            var processProductsScreenshotsTask = taskStatusController.Create(processUpdatesTask, "Process product screenshots");

            foreach (var id in screenshotsDataController.EnumerateIds())
            {
                var productScreenshots = await screenshotsDataController.GetByIdAsync(id);

                if (productScreenshots == null)
                {
                    taskStatusController.Warn(processProductsScreenshotsTask, $"Product {id} doesn't have screenshots");
                    continue;
                }

                taskStatusController.UpdateProgress(
                    processUpdatesTask, 
                    ++current, 
                    total,
                    productScreenshots.Title);

                var currentProductScreenshotSources = new List<string>();

                foreach (var uri in productScreenshots.Uris)
                {
                    var sourceUri = screenshotUriController.ExpandUri(uri);
                    var destinationUri = Path.Combine(
                        screenshotsDirectoryDelegate.GetDirectory(),
                        Path.GetFileName(sourceUri));

                    if (fileController.Exists(destinationUri)) continue;

                    currentProductScreenshotSources.Add(sourceUri);
                }

                if (currentProductScreenshotSources.Any())
                    screenshotsSources.Add(id, currentProductScreenshotSources);
            }

            taskStatusController.Complete(processProductsScreenshotsTask);
            taskStatusController.Complete(processUpdatesTask);

            return screenshotsSources;
        }
    }
}
