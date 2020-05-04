using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using Interfaces.Delegates.Values;
using GOG.Interfaces.Delegates.GetDownloadSources;

namespace GOG.Delegates.GetDownloadSources
{
    public abstract class GetProductCoreImagesDownloadSourcesAsyncDelegate<T> : IGetDownloadSourcesAsyncDelegate
        where T : ProductCore
    {
        private readonly IGetDataAsyncDelegate<T, long> getDataByIdAsyncDelegate;
        private readonly IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate;
        private readonly IFormatDelegate<string, string> formatImagesUriDelegate;
        private readonly IGetValueDelegate<string, T> getImageUriDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        protected GetProductCoreImagesDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<T, long> getDataByIdAsyncDelegate,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetValueDelegate<string, T> getImageUriDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllUpdatedAsyncDelegate = itemizeAllUpdatedAsyncDelegate;
            this.getDataByIdAsyncDelegate = getDataByIdAsyncDelegate;
            this.formatImagesUriDelegate = formatImagesUriDelegate;
            this.getImageUriDelegate = getImageUriDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            startDelegate.Start("Get download sources");

            var productImageSources = new Dictionary<long, IList<string>>();

            await foreach (var id in itemizeAllUpdatedAsyncDelegate.ItemizeAllAsync())
            {
                setProgressDelegate.SetProgress();

                var productCore = await getDataByIdAsyncDelegate.GetDataAsync(id);

                // not all updated products can be found with all dataControllers
                if (productCore == null) continue;

                var imageSources = new List<string>
                {
                    formatImagesUriDelegate.Format(
                        getImageUriDelegate.GetValue(productCore))
                };

                if (!productImageSources.ContainsKey(id))
                    productImageSources.Add(id, new List<string>());

                foreach (var source in imageSources)
                    productImageSources[id].Add(source);
            }

            completeDelegate.Complete();

            return productImageSources;
        }
    }
}