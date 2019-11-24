using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Network;

using Interfaces.Status;

using Interfaces.Models.Entities;

using Models.ProductScreenshots;

using GOG.Models;

using GOG.Interfaces.Delegates.UpdateScreenshots;

namespace GOG.Delegates.UpdateScreenshots
{
    public class UpdateScreenshotsAsyncDelegate : IUpdateScreenshotsAsyncDelegate<Product>
    {
        readonly IGetValueDelegate<string> getUpdateUriDelegate;
        readonly IDataController<ProductScreenshots> screenshotsDataController;
        readonly INetworkController networkController;
        readonly IItemizeDelegate<string, string> itemizeScreenshotsDelegates;

        readonly IStatusController statusController;
        public UpdateScreenshotsAsyncDelegate(
            IGetValueDelegate<string> getUpdateUriDelegate,
            IDataController<ProductScreenshots> screenshotsDataController,
            INetworkController networkController,
            IItemizeDelegate<string, string> itemizeScreenshotsDelegates,
            IStatusController statusController)
        {
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.screenshotsDataController = screenshotsDataController;
            this.networkController = networkController;
            this.itemizeScreenshotsDelegates = itemizeScreenshotsDelegates;
            this.statusController = statusController;
        }

        public async Task UpdateScreenshotsAsync(Product product, IStatus status)
        {
            var requestProductPageTask = await statusController.CreateAsync(status, "Request product page containing screenshots information");
            var productPageUri = string.Format(getUpdateUriDelegate.GetValue(), product.Url);
            var productPageContent = await networkController.GetResourceAsync(requestProductPageTask, productPageUri);
            await statusController.CompleteAsync(requestProductPageTask);

            var extractScreenshotsTask = await statusController.CreateAsync(status, "Exract screenshots from the page");
            var extractedProductScreenshots = itemizeScreenshotsDelegates.Itemize(productPageContent);

            if (extractedProductScreenshots == null) return;

            var productScreenshots = new ProductScreenshots
            {
                Id = product.Id,
                Title = product.Title,
                Uris = new List<string>(extractedProductScreenshots)
            };
            await statusController.CompleteAsync(extractScreenshotsTask);

            var updateProductScreenshotsTask = await statusController.CreateAsync(status, "Add product screenshots");
            await screenshotsDataController.UpdateAsync(productScreenshots, updateProductScreenshotsTask);
            await statusController.CompleteAsync(updateProductScreenshotsTask);
        }
    }
}
