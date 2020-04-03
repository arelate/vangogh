using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.GetData;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using GOG.Interfaces.Delegates.UpdateScreenshots;

using Attributes;

using Models.ProductTypes;

using GOG.Models;

namespace GOG.Delegates.UpdateScreenshots
{
    public class UpdateScreenshotsAsyncDelegate : IUpdateScreenshotsAsyncDelegate<Product>
    {
        readonly IGetValueDelegate<string> getUpdateUriDelegate;
        readonly IDataController<ProductScreenshots> screenshotsDataController;
        private readonly IGetDataAsyncDelegate<string> getUriDataAsyncDelegate;
        readonly IItemizeDelegate<string, string> itemizeScreenshotsDelegates;

        readonly IActionLogController actionLogController;

        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetScreenshotsUpdateUriDelegate,Delegates",
            "Controllers.Data.ProductTypes.ProductScreenshotsDataController,Controllers",
            "GOG.Delegates.GetData.Network.GetUriDataRateLimitedAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeScreenshotsDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdateScreenshotsAsyncDelegate(
            IGetValueDelegate<string> getUpdateUriDelegate,
            IDataController<ProductScreenshots> screenshotsDataController,
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeScreenshotsDelegates,
            IActionLogController actionLogController)
        {
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.screenshotsDataController = screenshotsDataController;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.itemizeScreenshotsDelegates = itemizeScreenshotsDelegates;
            this.actionLogController = actionLogController;
        }

        public async Task UpdateScreenshotsAsync(Product product)
        {
            actionLogController.StartAction("Request product page containing screenshots information");
            var productPageUri = string.Format(getUpdateUriDelegate.GetValue(), product.Url);
            var productPageContent = await getUriDataAsyncDelegate.GetDataAsync(productPageUri);
            actionLogController.CompleteAction();

            actionLogController.StartAction("Exract screenshots from the page");
            var extractedProductScreenshots = itemizeScreenshotsDelegates.Itemize(productPageContent);

            if (extractedProductScreenshots == null) return;

            var productScreenshots = new ProductScreenshots
            {
                Id = product.Id,
                Title = product.Title,
                Uris = new List<string>(extractedProductScreenshots)
            };
            actionLogController.CompleteAction();

            actionLogController.StartAction("Add product screenshots");
            await screenshotsDataController.UpdateAsync(productScreenshots);
            actionLogController.CompleteAction();
        }
    }
}
