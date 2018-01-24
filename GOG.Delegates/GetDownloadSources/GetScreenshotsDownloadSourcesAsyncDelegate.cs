using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.File;

using Interfaces.Delegates.Format;
using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.ProductScreenshots;

using GOG.Interfaces.Delegates.GetDownloadSources;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetScreenshotsDownloadSourcesAsyncDelegate : IGetDownloadSourcesAsyncDelegate
    {
        private IDataController<long, ProductScreenshots> screenshotsDataController;
        private IFormatDelegate<string, string> formatScreenshotsUriDelegate;
        private IGetDirectoryDelegate screenshotsDirectoryDelegate;
        private IFileController fileController;
        private IStatusController statusController;

        public GetScreenshotsDownloadSourcesAsyncDelegate(
            IDataController<long, ProductScreenshots> screenshotsDataController,
            IFormatDelegate<string, string> formatScreenshotsUriDelegate,
            IGetDirectoryDelegate screenshotsDirectoryDelegate,
            IFileController fileController,
            IStatusController statusController)
        {
            this.screenshotsDataController = screenshotsDataController;
            this.formatScreenshotsUriDelegate = formatScreenshotsUriDelegate;
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

            foreach (var id in await screenshotsDataController.ItemizeAllAsync(processProductsScreenshotsTask))
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
                    var sourceUri = formatScreenshotsUriDelegate.Format(uri);
                    var destinationUri = Path.Combine(
                        screenshotsDirectoryDelegate.GetDirectory(string.Empty),
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
