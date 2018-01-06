using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Enumeration;
using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDownloadSources;

using GOG.Models;

namespace GOG.Delegates.GetDownloadSources
{
    public class GetManualUrlDownloadSourcesAsyncDelegate : IGetDownloadSourcesAsyncDelegate
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsManualUrlEnumerationController;
        private IStatusController statusController;

        public GetManualUrlDownloadSourcesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateAsyncDelegate<GameDetails> gameDetailsManualUrlEnumerationController,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsManualUrlEnumerationController = gameDetailsManualUrlEnumerationController;
            this.statusController = statusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(IStatus status)
        {
            var getDownloadSourcesStatus = await statusController.CreateAsync(status, "Get download sources");

            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();
            var current = 0;

            foreach (var id in await updatedDataController.EnumerateIdsAsync(getDownloadSourcesStatus))
            {
                await statusController.UpdateProgressAsync(
                    getDownloadSourcesStatus,
                    ++current,
                    await updatedDataController.CountAsync(getDownloadSourcesStatus),
                    id.ToString());

                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, getDownloadSourcesStatus);

                if (!gameDetailsDownloadSources.ContainsKey(id))
                    gameDetailsDownloadSources.Add(id, new List<string>());

                foreach (var manualUrl in await gameDetailsManualUrlEnumerationController.EnumerateAsync(gameDetails, status))
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
