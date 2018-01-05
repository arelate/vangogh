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
            var processUpdatesTask = await statusController.CreateAsync(status, "Process screenshots updates");

            var screenshotsSources = new Dictionary<long, IList<string>>();
            var current = 0;
            var total = await screenshotsDataController.CountAsync(processUpdatesTask);

            var processProductsScreenshotsTask = await statusController.CreateAsync(processUpdatesTask, "Process product screenshots");

            foreach (var id in await screenshotsDataController.EnumerateIdsAsync(processProductsScreenshotsTask))
            {
                var productScreenshots = await screenshotsDataController.GetByIdAsync(id, processProductsScreenshotsTask);

                if (productScreenshots == null)
                {
                    await statusController.WarnAsync(processProductsScreenshotsTask, $"Product {id} doesn't have screenshots");
                    continue;
                }

                await statusController.UpdateProgressAsync(
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

            await statusController.CompleteAsync(processProductsScreenshotsTask);
            await statusController.CompleteAsync(processUpdatesTask);

            return screenshotsSources;
        }
    }
}
