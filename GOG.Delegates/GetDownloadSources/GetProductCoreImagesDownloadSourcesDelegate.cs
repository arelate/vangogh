using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Delegates.Format;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Interfaces.Status;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetImageUri;
using GOG.Interfaces.Delegates.GetDownloadSources;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetProductCoreImagesDownloadSourcesAsyncDelegate<T> : IGetDownloadSourcesAsyncDelegate
        where T : ProductCore
    {
        private IDataController<T> dataController;
        private IItemizeAllAsyncDelegate<long> itemizeAllProductsAsyncDelegate;
        private IFormatDelegate<string, string> formatImagesUriDelegate;
        private IGetImageUriDelegate<T> getImageUriDelegate;
        private IStatusController statusController;

        public GetProductCoreImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllProductsAsyncDelegate,
            IDataController<T> dataController,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<T> getImageUriDelegate,
            IStatusController statusController)
        {
            this.itemizeAllProductsAsyncDelegate = itemizeAllProductsAsyncDelegate;
            this.dataController = dataController;
            this.formatImagesUriDelegate = formatImagesUriDelegate;
            this.getImageUriDelegate = getImageUriDelegate;
            this.statusController = statusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(IStatus status)
        {
            var getDownloadSourcesStatus = await statusController.CreateAsync(status, "Get download sources");

            var productImageSources = new Dictionary<long, IList<string>>();
            var productIds = await itemizeAllProductsAsyncDelegate.ItemizeAllAsync(getDownloadSourcesStatus);
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
                    formatImagesUriDelegate.Format(
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
