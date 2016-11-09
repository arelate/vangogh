using System.Collections.Generic;

using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Download.Dependencies.ProductFiles
{
    public class ProductExtrasDownloadSourcesController : ProductDownloadSourcesController
    {
        public ProductExtrasDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController):
            base (
                updatedDataController,
                gameDetailsDataController)
        {
            // ...
        }

        internal override List<string> GetDownloadSources(GameDetails gameDetails)
        {
            var downloadSources = new List<string>();

            foreach (var extraDownloadEntry in gameDetails.Extras)
                downloadSources.Add(extraDownloadEntry.ManualUrl);

            return downloadSources;
        }
    }
}
