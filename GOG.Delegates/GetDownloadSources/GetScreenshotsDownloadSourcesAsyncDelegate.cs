using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.File;

using Interfaces.Delegates.Format;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDownloadSources;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetScreenshotsDownloadSourcesAsyncDelegate : IGetDownloadSourcesAsyncDelegate
    {
        readonly IDataController<ProductScreenshots> screenshotsDataController;
        readonly IFormatDelegate<string, string> formatScreenshotsUriDelegate;
        readonly IGetDirectoryDelegate screenshotsDirectoryDelegate;
        readonly IFileController fileController;
        readonly IActionLogController actionLogController;

		[Dependencies(
			"Controllers.Data.ProductTypes.ProductScreenshotsDataController,Controllers",
			"Delegates.Format.Uri.FormatScreenshotsUriDelegate,Delegates",
			"Delegates.GetDirectory.ProductTypes.GetScreenshotsDirectoryDelegate,Delegates",
			"Controllers.File.FileController,Controllers",
			"Controllers.Logs.ActionLogController,Controllers")]
        public GetScreenshotsDownloadSourcesAsyncDelegate(
            IDataController<ProductScreenshots> screenshotsDataController,
            IFormatDelegate<string, string> formatScreenshotsUriDelegate,
            IGetDirectoryDelegate screenshotsDirectoryDelegate,
            IFileController fileController,
            IActionLogController actionLogController)
        {
            this.screenshotsDataController = screenshotsDataController;
            this.formatScreenshotsUriDelegate = formatScreenshotsUriDelegate;
            this.screenshotsDirectoryDelegate = screenshotsDirectoryDelegate;
            this.fileController = fileController;
            this.actionLogController = actionLogController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            actionLogController.StartAction("Process screenshots updates");

            var screenshotsSources = new Dictionary<long, IList<string>>();

            await foreach (var productScreenshots in screenshotsDataController.ItemizeAllAsync())
            {
                if (productScreenshots == null)
                {
                    // await statusController.WarnAsync(processProductsScreenshotsTask, $"Product {productScreenshots.Id} doesn't have screenshots");
                    continue;
                }

                actionLogController.IncrementActionProgress();

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

            actionLogController.CompleteAction();

            return screenshotsSources;
        }
    }
}
