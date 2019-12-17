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
    public abstract class GetProductCoreImagesDownloadSourcesAsyncDelegate<T> : IGetDownloadSourcesAsyncDelegate
        where T : ProductCore
    {
        readonly IDataController<T> dataController;
        readonly IDataController<long> updatedDataController;
        readonly IFormatDelegate<string, string> formatImagesUriDelegate;
        readonly IGetImageUriDelegate<T> getImageUriDelegate;
        readonly IStatusController statusController;

        public GetProductCoreImagesDownloadSourcesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<T> dataController,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<T> getImageUriDelegate,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.dataController = dataController;
            this.formatImagesUriDelegate = formatImagesUriDelegate;
            this.getImageUriDelegate = getImageUriDelegate;
            this.statusController = statusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(IStatus status)
        {
            var getDownloadSourcesStatus = await statusController.CreateAsync(status, "Get download sources");

            var productImageSources = new Dictionary<long, IList<string>>();
            var count = await updatedDataController.CountAsync(getDownloadSourcesStatus);
            var current = 0;

            await foreach (var id in updatedDataController.ItemizeAllAsync(getDownloadSourcesStatus))
            {
                await statusController.UpdateProgressAsync(
                    getDownloadSourcesStatus,
                    ++current,
                    count,
                    id.ToString());

                var productCore = await dataController.GetByIdAsync(id, getDownloadSourcesStatus);

                // not all updated products can be found with all dataControllers
                if (productCore == null) continue;

                var imageSources = new List<string> {
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
