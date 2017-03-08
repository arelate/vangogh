using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductCore;

namespace GOG.Controllers.DownloadSources
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

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(ITaskStatus taskStatus)
        {
            var productImageSources = new Dictionary<long, IList<string>>();

            foreach (var id in updateDataController.EnumerateIds())
            {
                var productCore = await dataController.GetByIdAsync(id);

                // not all updated products can be found with all dataControllers
                if (productCore == null) continue;

                var imageSources = new List<string>() { imageUriController.ExpandUri(GetImageUri(productCore)) };

                if (!productImageSources.ContainsKey(id))
                    productImageSources.Add(id, new List<string>());

                foreach (var source in imageSources)
                    productImageSources[id].Add(source);
            }

            return productImageSources;
        }

        internal virtual string GetImageUri(T productCore)
        {
            throw new System.NotImplementedException();
        }
    }
}
