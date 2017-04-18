using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GOG.Interfaces.UpdateScreenshots;

using Interfaces.UpdateUri;
using Interfaces.Data;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Status;

using GOG.Models;

using Models.FlightPlan;
using Models.ProductScreenshots;

namespace GOG.Controllers.UpdateScreenshots
{
    public class UpdateProductScreenshotsDelegate : IUpdateScreenshotsDelegate<Product>
    {

        private IGetUpdateUriDelegate<string> getUpdateUriDelegate;
        private IDataController<ProductScreenshots> screenshotsDataController;
        private INetworkController networkController;
        private IStringExtractionController screenshotExtractionController;

        private IStatusController statusController;
        public UpdateProductScreenshotsDelegate(
            IGetUpdateUriDelegate<string> getUpdateUriDelegate,
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

        public async Task UpdateProductScreenshots(Product product, IStatus status)
        {
            var requestProductPageTask = statusController.Create(status, "Request product page containing screenshots information");
            var productPageUri = string.Format(getUpdateUriDelegate.GetUpdateUri(Parameters.Screenshots), product.Url);
            var productPageContent = await networkController.Get(requestProductPageTask, productPageUri);
            statusController.Complete(requestProductPageTask);

            var extractScreenshotsTask = statusController.Create(status, "Exract screenshots from the page");
            var extractedProductScreenshots = screenshotExtractionController.ExtractMultiple(productPageContent);

            if (extractedProductScreenshots == null) return;

            var productScreenshots = new ProductScreenshots()
            {
                Id = product.Id,
                Title = product.Title,
                Uris = new List<string>(extractedProductScreenshots)
            };
            statusController.Complete(extractScreenshotsTask);

            var updateProductScreenshotsTask = statusController.Create(status, "Add product screenshots");
            await screenshotsDataController.UpdateAsync(updateProductScreenshotsTask, productScreenshots);
            statusController.Complete(updateProductScreenshotsTask);
        }
    }
}
