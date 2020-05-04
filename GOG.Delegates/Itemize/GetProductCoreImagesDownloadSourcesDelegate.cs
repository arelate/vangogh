using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace GOG.Delegates.Itemize
{
    public abstract class ItemizeAllProductCoreImagesDownloadSourcesAsyncDelegate<T> : 
        IItemizeAllAsyncDelegate<(long, IList<string>)>
        where T : ProductCore
    {
        private readonly IGetDataAsyncDelegate<T, long> getDataByIdAsyncDelegate;
        private readonly IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate;
        private readonly IFormatDelegate<string, string> formatImagesUriDelegate;
        private readonly IGetValueDelegate<string, T> getImageUriDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        protected ItemizeAllProductCoreImagesDownloadSourcesAsyncDelegate(
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

        public async IAsyncEnumerable<(long, IList<string>)> ItemizeAllAsync()
        {
            startDelegate.Start("Get download sources");

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

                yield return (id, imageSources);
            }

            completeDelegate.Complete();
        }
    }
}