using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.ProductTypes;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductExtras
{
    public class ProductExtrasDownloadSourcesController : IDownloadSourcesController
    {
        private IProductTypeStorageController productTypeStorageController;

        public ProductExtrasDownloadSourcesController(
            IProductTypeStorageController productTypeStorageController)
        {
            this.productTypeStorageController = productTypeStorageController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var gameDetailsCollection = await productTypeStorageController.Pull<GameDetails>(ProductTypes.GameDetails);

            var extrasDownloadSources = new Dictionary<long, IList<string>>();

            foreach (var gameDetails in gameDetailsCollection)
            {
                var extrasSources = new List<string>();

                foreach (var extraDownloadEntry in gameDetails.Extras)
                    extrasSources.Add(extraDownloadEntry.ManualUrl);

                extrasDownloadSources.Add(gameDetails.Id, extrasSources);
            }

            return extrasDownloadSources;
        }
    }
}
