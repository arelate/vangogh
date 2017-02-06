using Interfaces.ImageUri;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Sources
{
    public class ProductsImagesDownloadSourcesController : ProductCoreImagesDownloadSourcesController<Product>
    {
        public ProductsImagesDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<Product> productsDataController,
            IImageUriController imageUriController): 
            base(
                updatedDataController,
                productsDataController, 
                imageUriController)
        {
            // ...
        }

        internal override string GetImageUri(Product productCore)
        {
            return productCore.Image;
        }
    }
}
