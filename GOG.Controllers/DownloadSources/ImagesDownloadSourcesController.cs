using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
        private IEnumerateIdsAsyncDelegate productEnumerateDelegate;
        private IExpandImageUriDelegate expandImageUriDelegate;
        private IGetImageUriDelegate<T> getImageUriDelegate;
        private IStatusController statusController;

        public ProductCoreImagesDownloadSourcesController(
            IEnumerateIdsAsyncDelegate productEnumerateDelegate,
            IDataController<T> dataController,
            IExpandImageUriDelegate expandImageUriDelegate,
            IGetImageUriDelegate<T> getImageUriDelegate,
            IStatusController statusController)
        {
            this.productEnumerateDelegate = productEnumerateDelegate;
            this.dataController = dataController;
            this.expandImageUriDelegate = expandImageUriDelegate;
            this.getImageUriDelegate = getImageUriDelegate;
            this.statusController = statusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(IStatus status)
        {
            var getDownloadSourcesStatus = await statusController.CreateAsync(status, "Get download sources");

            var productImageSources = new Dictionary<long, IList<string>>();
            var productIds = await productEnumerateDelegate.EnumerateIdsAsync(getDownloadSourcesStatus);
            var current = 0;

            foreach (var id in productIds)
            {
                await statusController.UpdateProgressAsync(
                    getDownloadSourcesStatus,
                    ++current,
                    productIds.Count(),
                    id.ToString());

                var productCore = await dataController.GetByIdAsync(id, getDownloadSourcesStatus);

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

            await statusController.CompleteAsync(getDownloadSourcesStatus);

            return productImageSources;
        }
    }
}
