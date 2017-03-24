using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.Controllers.DownloadSources
{
    public class ManualUrlDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<GameDetails> gameDetailsManualUrlEnumerationController;

        public ManualUrlDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<GameDetails> gameDetailsManualUrlEnumerationController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsManualUrlEnumerationController = gameDetailsManualUrlEnumerationController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(ITaskStatus taskStatus)
        {
            var gameDetailsDownloadSources = new Dictionary<long, IList<string>>();

            foreach (var id in updatedDataController.EnumerateIds())
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                if (!gameDetailsDownloadSources.ContainsKey(id))
                    gameDetailsDownloadSources.Add(id, new List<string>());

                foreach (var manualUrl in gameDetailsManualUrlEnumerationController.Enumerate(gameDetails))
                {
                    if (!gameDetailsDownloadSources[id].Contains(manualUrl))
                        gameDetailsDownloadSources[id].Add(manualUrl);
                }
            }

            return gameDetailsDownloadSources;
        }
    }
}
