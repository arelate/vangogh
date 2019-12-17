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

using Attributes;

using Models.ProductScreenshots;

using GOG.Interfaces.Delegates.GetDownloadSources;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetScreenshotsDownloadSourcesAsyncDelegate : IGetDownloadSourcesAsyncDelegate
    {
        readonly IDataController<ProductScreenshots> screenshotsDataController;
        readonly IFormatDelegate<string, string> formatScreenshotsUriDelegate;
        readonly IGetDirectoryDelegate screenshotsDirectoryDelegate;
        readonly IFileController fileController;
        readonly IStatusController statusController;

		[Dependencies(
			"Controllers.Data.ProductTypes.ProductScreenshotsDataController,Controllers",
			"Delegates.Format.Uri.FormatScreenshotsUriDelegate,Delegates",
			"Delegates.GetDirectory.ProductTypes.GetScreenshotsDirectoryDelegate,Delegates",
			"Controllers.File.FileController,Controllers",
			"Controllers.Status.StatusController,Controllers")]
        public GetScreenshotsDownloadSourcesAsyncDelegate(
            IDataController<ProductScreenshots> screenshotsDataController,
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

            await foreach (var productScreenshots in screenshotsDataController.ItemizeAllAsync(processProductsScreenshotsTask))
            {
                if (productScreenshots == null)
                {
                    await statusController.WarnAsync(processProductsScreenshotsTask, $"Product {productScreenshots.Id} doesn't have screenshots");
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
                    screenshotsSources.Add(productScreenshots.Id, currentProductScreenshotSources);
            }

            await statusController.CompleteAsync(processProductsScreenshotsTask);
            await statusController.CompleteAsync(processUpdatesTask);

            return screenshotsSources;
        }
    }
}
