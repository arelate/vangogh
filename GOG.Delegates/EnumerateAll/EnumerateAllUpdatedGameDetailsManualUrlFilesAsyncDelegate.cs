using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Controllers.Data;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Delegates.EnumerateAll
{
    public class EnumerateAllUpdatedGameDetailsManualUrlFilesAsyncDelegate : IEnumerateAllAsyncDelegate<string>
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsFilesEnumerateDelegate;
        private IStatusController statusController;

        public EnumerateAllUpdatedGameDetailsManualUrlFilesAsyncDelegate(
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
            var enumerateUpdateGameDetailsFilesTask = await statusController.CreateAsync(status, "Enumerate updated gameDetails files");

            var updatedIds = await updatedDataController.EnumerateIdsAsync(enumerateUpdateGameDetailsFilesTask);
            var updatedIdsCount = await updatedDataController.CountAsync(enumerateUpdateGameDetailsFilesTask);
            var current = 0;

            var gameDetailsFiles = new List<string>();

            foreach (var id in updatedIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, enumerateUpdateGameDetailsFilesTask);

                await statusController.UpdateProgressAsync(
                    enumerateUpdateGameDetailsFilesTask,
                    ++current,
                    updatedIdsCount,
                    gameDetails.Title);

                gameDetailsFiles.AddRange(
                    await gameDetailsFilesEnumerateDelegate.EnumerateAsync(
                        gameDetails, 
                        enumerateUpdateGameDetailsFilesTask));
            }

            await statusController.CompleteAsync(enumerateUpdateGameDetailsFilesTask);

            return gameDetailsFiles;
        }
    }
}
