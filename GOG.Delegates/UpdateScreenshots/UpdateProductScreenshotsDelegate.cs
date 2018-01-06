using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Status;
using Interfaces.ContextDefinitions;

using Models.ProductScreenshots;

using GOG.Models;

using GOG.Interfaces.Delegates.GetUpdateUri;
using GOG.Interfaces.Delegates.UpdateScreenshots;

namespace GOG.Delegates.UpdateScreenshots
{
    public class UpdateScreenshotsAsyncDelegate : IUpdateScreenshotsAsyncDelegate<Product>
    {
        private IGetUpdateUriDelegate<Context> getUpdateUriDelegate;
        private IDataController<ProductScreenshots> screenshotsDataController;
        private INetworkController networkController;
        private IStringExtractionController screenshotExtractionController;

        private IStatusController statusController;
        public UpdateScreenshotsAsyncDelegate(
            IGetUpdateUriDelegate<Context> getUpdateUriDelegate,
            IDataController<ProductScreenshots> screenshotsDataController,
            INetworkController networkController,
            IStringExtractionController screenshotExtractionController,
            IStatusController statusController)
        {
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.screenshotsDataController = screenshotsDataController;
            this.networkController = networkController;
            this.screenshotExtractionController = screenshotExtractionController;
            this.statusController = statusController;
        }

        public async Task UpdateScreenshotsAsync(Product product, IStatus status)
        {
            var requestProductPageTask = await statusController.CreateAsync(status, "Request product page containing screenshots information");
            var productPageUri = string.Format(getUpdateUriDelegate.GetUpdateUri(Context.Screenshots), product.Url);
            var productPageContent = await networkController.GetAsync(requestProductPageTask, productPageUri);
            await statusController.CompleteAsync(requestProductPageTask);

            var extractScreenshotsTask = await statusController.CreateAsync(status, "Exract screenshots from the page");
            var extractedProductScreenshots = screenshotExtractionController.ExtractMultiple(productPageContent);

            if (extractedProductScreenshots == null) return;

            var productScreenshots = new ProductScreenshots()
            {
                Id = product.Id,
                Title = product.Title,
                Uris = new List<string>(extractedProductScreenshots)
            };
            await statusController.CompleteAsync(extractScreenshotsTask);

            var updateProductScreenshotsTask = await statusController.CreateAsync(status, "Add product screenshots");
            await screenshotsDataController.UpdateAsync(updateProductScreenshotsTask, productScreenshots);
            await statusController.CompleteAsync(updateProductScreenshotsTask);
        }
    }
}
