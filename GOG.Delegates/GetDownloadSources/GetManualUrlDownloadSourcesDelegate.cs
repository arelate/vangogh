using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;

using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDownloadSources;

using GOG.Models;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetManualUrlDownloadSourcesAsyncDelegate : IGetDownloadSourcesAsyncDelegate
    {
        readonly IIndexController<long> updatedDataController;
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncController;
        readonly IStatusController statusController;

        public GetManualUrlDownloadSourcesAsyncDelegate(
            IIndexController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncController,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsManualUrlsAsyncController = itemizeGameDetailsManualUrlsAsyncController;
            this.statusController = statusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(IStatus status)
        {
            var getDownloadSourcesStatus = await statusController.CreateAsync(status, "Get download sources");

            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();
            var current = 0;

            await foreach (var id in updatedDataController.ItemizeAllAsync(getDownloadSourcesStatus))
            {
                await statusController.UpdateProgressAsync(
                    getDownloadSourcesStatus,
                    ++current,
                    await updatedDataController.CountAsync(getDownloadSourcesStatus),
                    id.ToString());

                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, getDownloadSourcesStatus);

                if (!gameDetailsDownloadSources.ContainsKey(id))
                    gameDetailsDownloadSources.Add(id, new List<string>());

                foreach (var manualUrl in 
                    await itemizeGameDetailsManualUrlsAsyncController.ItemizeAsync(gameDetails, status))
                {
                    if (!gameDetailsDownloadSources[id].Contains(manualUrl))
                        gameDetailsDownloadSources[id].Add(manualUrl);
                }
            }

            await statusController.CompleteAsync(getDownloadSourcesStatus);

            return gameDetailsDownloadSources;
        }
    }
}
