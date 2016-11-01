using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductImages
{
    public class ProductsImagesDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<Product> productsDataController;
        private IImageUriController imageUriController;

        public ProductsImagesDownloadSourcesController(
            IDataController<Product> productsDataController,
            IImageUriController imageUriController)
        {
            this.productsDataController = productsDataController;
            this.imageUriController = imageUriController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var productImageSources = new Dictionary<long, IList<string>>();

            foreach (var id in productsDataController.EnumerateIds())
            {
                var product = await productsDataController.GetById(id);

                var imageSources = new List<string>() { imageUriController.ExpandUri(product.Image) };
                productImageSources.Add(product.Id, imageSources);
            }

            return productImageSources;
        }
    }
}
