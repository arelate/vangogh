using System.Collections.Generic;
using System.IO;
using System.Linq;
using Attributes;
using Delegates.Activities;
using Delegates.Format.Uri;
using Delegates.Itemizations.ProductTypes;
using Delegates.Values.Directories.ProductTypes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllScreenshotsDownloadSourcesAsyncDelegate : IItemizeAllAsyncDelegate<(long, IList<string>)>
    {
        private readonly IItemizeAllAsyncDelegate<ProductScreenshots> itemizeAllProductScreenshotsAsyncDelegate;
        private readonly IFormatDelegate<string, string> formatScreenshotsUriDelegate;
        private readonly IGetValueDelegate<string,string> screenshotsDirectoryDelegate;
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
        public ItemizeAllScreenshotsDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<ProductScreenshots> itemizeAllProductScreenshotsAsyncDelegate,
            IFormatDelegate<string, string> formatScreenshotsUriDelegate,
            IGetValueDelegate<string,string> screenshotsDirectoryDelegate,
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

        public async IAsyncEnumerable<(long, IList<string>)> ItemizeAllAsync()
        {
            startDelegate.Start("Process screenshots updates");

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
                        screenshotsDirectoryDelegate.GetValue(string.Empty),
                        Path.GetFileName(sourceUri));

                    if (File.Exists(destinationUri)) continue;

                    currentProductScreenshotSources.Add(sourceUri);
                }

                if (currentProductScreenshotSources.Any())
                    yield return (productScreenshots.Id, currentProductScreenshotSources);
            }

            completeDelegate.Complete();
        }
    }
}