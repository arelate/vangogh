using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;


using GOG.Interfaces.Delegates.GetDownloadSources;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetManualUrlDownloadSourcesAsyncDelegate : IGetDownloadSourcesAsyncDelegate
    {
        readonly IDataController<long> updatedDataController;
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncController;
        readonly IActionLogController actionLogController;

		[Dependencies(
			"Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.GameDetailsDataController,GOG.Controllers",
			"GOG.Delegates.Itemize.ItemizeGameDetailsManualUrlsAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]
        public GetManualUrlDownloadSourcesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncController,
            IActionLogController actionLogController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsManualUrlsAsyncController = itemizeGameDetailsManualUrlsAsyncController;
            this.actionLogController = actionLogController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            actionLogController.StartAction("Get download sources");

            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();

            await foreach (var id in updatedDataController.ItemizeAllAsync())
            {
                actionLogController.IncrementActionProgress();

                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                if (!gameDetailsDownloadSources.ContainsKey(id))
                    gameDetailsDownloadSources.Add(id, new List<string>());

                foreach (var manualUrl in 
                    await itemizeGameDetailsManualUrlsAsyncController.ItemizeAsync(gameDetails))
                {
                    if (!gameDetailsDownloadSources[id].Contains(manualUrl))
                        gameDetailsDownloadSources[id].Add(manualUrl);
                }
            }

            actionLogController.CompleteAction();

            return gameDetailsDownloadSources;
        }
    }
}
