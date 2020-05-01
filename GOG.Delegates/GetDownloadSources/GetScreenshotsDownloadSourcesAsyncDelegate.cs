using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.GetDownloadSources;
using Delegates.Itemize.ProductTypes;
using Delegates.Format.Uri;
using Delegates.GetDirectory.ProductTypes;
using Delegates.Activities;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetScreenshotsDownloadSourcesAsyncDelegate : IGetDownloadSourcesAsyncDelegate
    {
        private readonly IItemizeAllAsyncDelegate<ProductScreenshots> itemizeAllProductScreenshotsAsyncDelegate;
        private readonly IFormatDelegate<string, string> formatScreenshotsUriDelegate;
        private readonly IGetDirectoryDelegate screenshotsDirectoryDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ItemizeAllProductScreenshotsAsyncDelegate),
            typeof(FormatScreenshotsUriDelegate),
            typeof(GetScreenshotsDirectoryDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public GetScreenshotsDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<ProductScreenshots> itemizeAllProductScreenshotsAsyncDelegate,
            IFormatDelegate<string, string> formatScreenshotsUriDelegate,
            IGetDirectoryDelegate screenshotsDirectoryDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllProductScreenshotsAsyncDelegate = itemizeAllProductScreenshotsAsyncDelegate;
            this.formatScreenshotsUriDelegate = formatScreenshotsUriDelegate;
            this.screenshotsDirectoryDelegate = screenshotsDirectoryDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            startDelegate.Start("Process screenshots updates");

            var screenshotsSources = new Dictionary<long, IList<string>>();

            await foreach (var productScreenshots in itemizeAllProductScreenshotsAsyncDelegate.ItemizeAllAsync())
            {
                if (productScreenshots == null)
                    // await statusController.WarnAsync(processProductsScreenshotsTask, $"Product {productScreenshots.Id} doesn't have screenshots");
                    continue;

                setProgressDelegate.SetProgress();

                var currentProductScreenshotSources = new List<string>();

                foreach (var uri in productScreenshots.Uris)
                {
                    var sourceUri = formatScreenshotsUriDelegate.Format(uri);
                    var destinationUri = Path.Combine(
                        screenshotsDirectoryDelegate.GetDirectory(string.Empty),
                        Path.GetFileName(sourceUri));

                    if (File.Exists(destinationUri)) continue;

                    currentProductScreenshotSources.Add(sourceUri);
                }

                if (currentProductScreenshotSources.Any())
                    screenshotsSources.Add(productScreenshots.Id, currentProductScreenshotSources);
            }

            completeDelegate.Complete();

            return screenshotsSources;
        }
    }
}