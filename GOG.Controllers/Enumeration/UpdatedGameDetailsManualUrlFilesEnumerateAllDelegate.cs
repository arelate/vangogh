using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Data;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class UpdatedGameDetailsManualUrlFilesEnumerateAllDelegate : IEnumerateAllAsyncDelegate<string>
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsFilesEnumerateDelegate;
        private IStatusController statusController;

        public UpdatedGameDetailsManualUrlFilesEnumerateAllDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateAsyncDelegate<GameDetails> gameDetailsFilesEnumerateDelegate,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsFilesEnumerateDelegate = gameDetailsFilesEnumerateDelegate;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> EnumerateAllAsync(IStatus status)
        {
            var enumerateUpdateGameDetailsFilesTask = statusController.Create(status, "Enumerate updated gameDetails files");

            var updatedIds = updatedDataController.EnumerateIds();
            var updatedIdsCount = updatedDataController.Count();
            var current = 0;

            var gameDetailsFiles = new List<string>();

            foreach (var id in updatedIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                statusController.UpdateProgress(
                    enumerateUpdateGameDetailsFilesTask,
                    ++current,
                    updatedIdsCount,
                    gameDetails.Title);

                gameDetailsFiles.AddRange(await gameDetailsFilesEnumerateDelegate.EnumerateAsync(gameDetails));
            }

            statusController.Complete(enumerateUpdateGameDetailsFilesTask);

            return gameDetailsFiles;
        }
    }
}
