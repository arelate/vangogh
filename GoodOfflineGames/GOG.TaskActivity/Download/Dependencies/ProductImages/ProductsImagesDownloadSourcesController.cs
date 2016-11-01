using Interfaces.ImageUri;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductImages
{
    public class ProductsImagesDownloadSourcesController : ProductCoreImagesDownloadSourcesController<Product>
    {
        public ProductsImagesDownloadSourcesController(
            IDataController<Product> productsDataController,
            IImageUriController imageUriController): 
            base(
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
