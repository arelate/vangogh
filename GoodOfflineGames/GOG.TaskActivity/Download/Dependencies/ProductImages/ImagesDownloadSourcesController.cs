using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;

using Models.ProductCore;

namespace GOG.TaskActivities.Download.Dependencies.ProductImages
{
    public abstract class ProductCoreImagesDownloadSourcesController<T> : IDownloadSourcesController 
        where T: ProductCore
    {
        private IDataController<T> dataController;
        private IDataController<long> updateDataController;
        private IImageUriController imageUriController;

        public ProductCoreImagesDownloadSourcesController(
            IDataController<long> updateDataController,
            IDataController<T> dataController,
            IImageUriController imageUriController)
        {
            this.updateDataController = updateDataController;
            this.dataController = dataController;
            this.imageUriController = imageUriController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var productImageSources = new Dictionary<long, IList<string>>();

            foreach (var id in updateDataController.EnumerateIds())
            {
                var productCore = await dataController.GetById(id);

                var imageSources = new List<string>() { imageUriController.ExpandUri(GetImageUri(productCore)) };
                productImageSources.Add(productCore.Id, imageSources);
            }

            return productImageSources;
        }

        internal virtual string GetImageUri(T productCore)
        {
            throw new System.NotImplementedException();
        }
    }
}
