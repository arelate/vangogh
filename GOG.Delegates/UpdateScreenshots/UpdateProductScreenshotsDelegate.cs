using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.UpdateScreenshots;
using Attributes;
using Models.ProductTypes;
using GOG.Models;
using Delegates.GetValue.Uri.ProductTypes;
using Delegates.Data.Models.ProductTypes;
using Delegates.Activities;

namespace GOG.Delegates.UpdateScreenshots
{
    public class UpdateScreenshotsAsyncDelegate : IUpdateScreenshotsAsyncDelegate<Product>
    {
        private readonly IGetValueDelegate<string> getUpdateUriDelegate;
        private readonly IUpdateAsyncDelegate<ProductScreenshots> updateProductScreenshotsAsyncDelegate;
        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;
        private readonly IItemizeDelegate<string, string> itemizeScreenshotsDelegates;

        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GetScreenshotsUpdateUriDelegate),
            typeof(UpdateProductScreenshotsAsyncDelegate),
            typeof(GOG.Delegates.Data.Network.GetUriDataRateLimitedAsyncDelegate),
            typeof(GOG.Delegates.Itemize.ItemizeScreenshotsDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public UpdateScreenshotsAsyncDelegate(
            IGetValueDelegate<string> getUpdateUriDelegate,
            IUpdateAsyncDelegate<ProductScreenshots> updateProductScreenshotsAsyncDelegate,
            IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeScreenshotsDelegates,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.updateProductScreenshotsAsyncDelegate = updateProductScreenshotsAsyncDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.itemizeScreenshotsDelegates = itemizeScreenshotsDelegates;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task UpdateScreenshotsAsync(Product product)
        {
            startDelegate.Start("Request product page containing screenshots information");
            var productPageUri = string.Format(getUpdateUriDelegate.GetValue(), product.Url);
            var productPageContent = await getUriDataAsyncDelegate.GetDataAsync(productPageUri);
            completeDelegate.Complete();

            startDelegate.Start("Exract screenshots from the page");
            var extractedProductScreenshots = itemizeScreenshotsDelegates.Itemize(productPageContent);

            if (extractedProductScreenshots == null) return;

            var productScreenshots = new ProductScreenshots
            {
                Id = product.Id,
                Title = product.Title,
                Uris = new List<string>(extractedProductScreenshots)
            };
            completeDelegate.Complete();

            startDelegate.Start("Add product screenshots");
            await updateProductScreenshotsAsyncDelegate.UpdateAsync(productScreenshots);
            completeDelegate.Complete();
        }
    }
}