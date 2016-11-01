using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductExtras
{
    public class ProductExtrasDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<GameDetails> gameDetailsDataController;

        public ProductExtrasDownloadSourcesController(IDataController<GameDetails> gameDetailsDataController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var extrasDownloadSources = new Dictionary<long, IList<string>>();

            foreach (var id in gameDetailsDataController.EnumerateIds())
            {
                var gameDetails = await gameDetailsDataController.GetById(id);
                var extrasSources = new List<string>();

                foreach (var extraDownloadEntry in gameDetails.Extras)
                    extrasSources.Add(extraDownloadEntry.ManualUrl);

                extrasDownloadSources.Add(gameDetails.Id, extrasSources);
            }

            return extrasDownloadSources;
        }
    }
}
