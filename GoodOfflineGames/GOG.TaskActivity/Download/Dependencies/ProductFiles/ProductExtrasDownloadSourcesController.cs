using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductFiles
{
    public class ProductExtrasDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<GameDetails> gameDetailsDataController;
        private IDataController<long> updatedDataController;

        public ProductExtrasDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var extrasDownloadSources = new Dictionary<long, IList<string>>();

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var gameDetails = await gameDetailsDataController.GetById(id);

                var downloadSources = new List<string>();

                foreach (var extraDownloadEntry in gameDetails.Extras)
                    downloadSources.Add(extraDownloadEntry.ManualUrl);

                if (!extrasDownloadSources.ContainsKey(id))
                    extrasDownloadSources.Add(id, new List<string>());

                foreach (var source in downloadSources)
                    if (!extrasDownloadSources[gameDetails.Id].Contains(source))
                        extrasDownloadSources[gameDetails.Id].Add(source);
            }

            return extrasDownloadSources;
        }
    }
}
