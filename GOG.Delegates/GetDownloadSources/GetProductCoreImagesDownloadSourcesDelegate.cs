using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Itemize;
using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.GetImageUri;
using GOG.Interfaces.Delegates.GetDownloadSources;

namespace GOG.Delegates.GetDownloadSources
{
    public abstract class GetProductCoreImagesDownloadSourcesAsyncDelegate<T> : IGetDownloadSourcesAsyncDelegate
        where T : ProductCore
    {
        private readonly IDataController<T> dataController;
        private readonly IDataController<long> updatedDataController;
        private readonly IFormatDelegate<string, string> formatImagesUriDelegate;
        private readonly IGetImageUriDelegate<T> getImageUriDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public GetProductCoreImagesDownloadSourcesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<T> dataController,
            IFormatDelegate<string, string> formatImagesUriDelegate,
            IGetImageUriDelegate<T> getImageUriDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.updatedDataController = updatedDataController;
            this.dataController = dataController;
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

            await foreach (var id in updatedDataController.ItemizeAllAsync())
            {
                setProgressDelegate.SetProgress();

                var productCore = await dataController.GetByIdAsync(id);

                // not all updated products can be found with all dataControllers
                if (productCore == null) continue;

                var imageSources = new List<string>
                {
                    formatImagesUriDelegate.Format(
                        getImageUriDelegate.GetImageUri(productCore))
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