using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Data;

using GOG.Models;

using Models.Uris;

namespace GOG.TaskActivities.Download.Dependencies.ProductFiles
{
    public abstract class ProductDownloadSourcesController : IDownloadSourcesController
    {
        internal IDataController<long> updatedDataController;
        internal IDataController<GameDetails> gameDetailsDataController;

        public ProductDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);
                if (gameDetails == null) continue;

                var downloadSources = GetDownloadSources(gameDetails);

                if (!gameDetailsDownloadSources.ContainsKey(id))
                    gameDetailsDownloadSources.Add(id, new List<string>());

                foreach (var source in downloadSources)
                {
                    var absoluteSourceUri = string.Format(Uris.Paths.ProductFiles.FullUriTemplate, source);

                    if (!gameDetailsDownloadSources[gameDetails.Id].Contains(absoluteSourceUri))
                        gameDetailsDownloadSources[gameDetails.Id].Add(absoluteSourceUri);
                }
            }

            return gameDetailsDownloadSources;
        }

        internal virtual List<string> GetDownloadSources(GameDetails gameDetails)
        {
            throw new NotImplementedException();
        }
    }
}
