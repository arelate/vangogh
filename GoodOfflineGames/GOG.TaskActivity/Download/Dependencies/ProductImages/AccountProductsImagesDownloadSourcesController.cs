using Interfaces.ImageUri;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductImages
{
    public class AccountProductsImagesDownloadSourcesController : ProductCoreImagesDownloadSourcesController<AccountProduct>
    {
        public AccountProductsImagesDownloadSourcesController(
            IDataController<AccountProduct> accountProductsDataController,
            IImageUriController imageUriController):
            base(
                accountProductsDataController, 
                imageUriController)
        {
            // ...
        }

        internal override string GetImageUri(AccountProduct productCore)
        {
            return productCore.Image;
        }
    }
}
