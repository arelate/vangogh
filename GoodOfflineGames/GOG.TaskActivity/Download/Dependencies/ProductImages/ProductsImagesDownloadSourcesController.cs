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
        //private IProductTypeStorageController productStorageController;
        private IImageUriController imageUriController;

        public ProductsImagesDownloadSourcesController(
            //IProductTypeStorageController productStorageController,
            IImageUriController imageUriController)
        {
            //this.productStorageController = productStorageController;
            this.imageUriController = imageUriController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var products = new List<Product>();// await productStorageController.Pull<Product>(ProductTypes.Product);
            var productImageSources = new Dictionary<long, IList<string>>();

            foreach (var product in products)
            {
                var imageSources = new List<string>() { imageUriController.ExpandUri(product.Image) };
                productImageSources.Add(product.Id, imageSources);
            }

            return productImageSources;
        }
    }
}
