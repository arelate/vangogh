using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Controllers.DownloadSources
{
    public class ManualUrlDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsManualUrlEnumerationController;
        private IStatusController statusController;

        public ManualUrlDownloadSourcesController(
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
            var getDownloadSourcesStatus = statusController.Create(status, "Get download sources");

            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();
            var current = 0;

            foreach (var id in await updatedDataController.EnumerateIdsAsync(getDownloadSourcesStatus))
            {
                statusController.UpdateProgress(
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

            statusController.Complete(getDownloadSourcesStatus);

            return gameDetailsDownloadSources;
        }
    }
}
