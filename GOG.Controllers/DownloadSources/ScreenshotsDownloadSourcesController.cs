using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;
using Interfaces.Status;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.File;

using Models.ProductScreenshots;

namespace GOG.Controllers.DownloadSources
{
    public class ScreenshotsDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IExpandImageUriDelegate expandScreenshotUriDelegate;
        private IGetDirectoryDelegate screenshotsDirectoryDelegate;
        private IFileController fileController;
        private IStatusController statusController;

        public ScreenshotsDownloadSourcesController(
            IDataController<ProductScreenshots> screenshotsDataController,
            IExpandImageUriDelegate expandScreenshotUriDelegate,
            IGetDirectoryDelegate screenshotsDirectoryDelegate,
            IFileController fileController,
            IStatusController statusController)
        {
            this.screenshotsDataController = screenshotsDataController;
            this.expandScreenshotUriDelegate = expandScreenshotUriDelegate;
            this.screenshotsDirectoryDelegate = screenshotsDirectoryDelegate;
            this.fileController = fileController;
            this.statusController = statusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(IStatus status)
        {
            var processUpdatesTask = statusController.Create(status, "Process screenshots updates");

            var screenshotsSources = new Dictionary<long, IList<string>>();
            var current = 0;
            var total = screenshotsDataController.Count();

            var processProductsScreenshotsTask = statusController.Create(processUpdatesTask, "Process product screenshots");

            foreach (var id in screenshotsDataController.EnumerateIds())
            {
                var productScreenshots = await screenshotsDataController.GetByIdAsync(id);

                if (productScreenshots == null)
                {
                    statusController.Warn(processProductsScreenshotsTask, $"Product {id} doesn't have screenshots");
                    continue;
                }

                statusController.UpdateProgress(
                    processUpdatesTask, 
                    ++current, 
                    total,
                    productScreenshots.Title);

                var currentProductScreenshotSources = new List<string>();

                foreach (var uri in productScreenshots.Uris)
                {
                    var sourceUri = expandScreenshotUriDelegate.ExpandImageUri(uri);
                    var destinationUri = Path.Combine(
                        screenshotsDirectoryDelegate.GetDirectory(),
                        Path.GetFileName(sourceUri));

                    if (fileController.Exists(destinationUri)) continue;

                    currentProductScreenshotSources.Add(sourceUri);
                }

                if (currentProductScreenshotSources.Any())
                    screenshotsSources.Add(id, currentProductScreenshotSources);
            }

            statusController.Complete(processProductsScreenshotsTask);
            statusController.Complete(processUpdatesTask);

            return screenshotsSources;
        }
    }
}
