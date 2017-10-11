using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;
using Interfaces.Status;

using Models.ProductCore;

namespace GOG.Controllers.DownloadSources
{
    public class ProductCoreImagesDownloadSourcesController<T> : IDownloadSourcesController
        where T : ProductCore
    {
        private IDataController<T> dataController;
        private IDataController<long> updateDataController;
        private IExpandImageUriDelegate expandImageUriDelegate;
        private IGetImageUriDelegate<T> getImageUriDelegate;

        public ProductCoreImagesDownloadSourcesController(
            IDataController<long> updateDataController,
            IDataController<T> dataController,
            IExpandImageUriDelegate expandImageUriDelegate,
            IGetImageUriDelegate<T> getImageUriDelegate)
        {
            this.updateDataController = updateDataController;
            this.dataController = dataController;
            this.expandImageUriDelegate = expandImageUriDelegate;
            this.getImageUriDelegate = getImageUriDelegate;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(IStatus status)
        {
            var productImageSources = new Dictionary<long, IList<string>>();

            foreach (var id in updateDataController.EnumerateIds())
            {
                var productCore = await dataController.GetByIdAsync(id);

                // not all updated products can be found with all dataControllers
                if (productCore == null) continue;

                var imageSources = new List<string>() {
                    expandImageUriDelegate.ExpandImageUri(
                        getImageUriDelegate.GetImageUri(productCore)) };

                if (!productImageSources.ContainsKey(id))
                    productImageSources.Add(id, new List<string>());

                foreach (var source in imageSources)
                    productImageSources[id].Add(source);
            }

            return productImageSources;
        }
    }
}
