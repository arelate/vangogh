using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;

using Interfaces.Status;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private IIndexController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsFilesAsyncDelegate;
        private IStatusController statusController;

        public ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate(
            IIndexController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsFilesAsyncDelegate,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsFilesAsyncDelegate = itemizeGameDetailsFilesAsyncDelegate;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> ItemizeAllAsync(IStatus status)
        {
            var enumerateUpdateGameDetailsFilesTask = await statusController.CreateAsync(status, "Enumerate updated gameDetails files");

            var updatedIds = await updatedDataController.ItemizeAllAsync(enumerateUpdateGameDetailsFilesTask);
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
                    await itemizeGameDetailsFilesAsyncDelegate.ItemizeAsync(
                        gameDetails, 
                        enumerateUpdateGameDetailsFilesTask));
            }

            await statusController.CompleteAsync(enumerateUpdateGameDetailsFilesTask);

            return gameDetailsFiles;
        }
    }
}
