using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.ImageUri;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductImages
{
    public class ProductsImagesDownloadSourcesController : IDownloadSourcesController
    {
        private IProductTypeStorageController productStorageController;
        private IImageUriController imageUriController;

        public ProductsImagesDownloadSourcesController(
            IProductTypeStorageController productStorageController,
            IImageUriController imageUriController)
        {
            this.productStorageController = productStorageController;
            this.imageUriController = imageUriController;
        }

        public async Task<IList<string>> GetDownloadSources()
        {
            var products = await productStorageController.Pull<Product>(ProductTypes.Product);

            var productImageSources = new List<string>();

            foreach (var product in products)
            {
                productImageSources.Add(imageUriController.ExpandUri(product.Image));
            }

            return productImageSources;
        }
    }
}
