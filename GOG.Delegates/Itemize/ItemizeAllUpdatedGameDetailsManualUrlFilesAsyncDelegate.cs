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
        readonly IIndexController<long> updatedDataController;
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsFilesAsyncDelegate;
        readonly IStatusController statusController;

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

        public async IAsyncEnumerable<string> ItemizeAllAsync(IStatus status)
        {
            var enumerateUpdateGameDetailsFilesTask = await statusController.CreateAsync(status, "Enumerate updated gameDetails files");

            var updatedIdsCount = await updatedDataController.CountAsync(enumerateUpdateGameDetailsFilesTask);
            var current = 0;
            
            await foreach (var id in updatedDataController.ItemizeAllAsync(enumerateUpdateGameDetailsFilesTask))
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, enumerateUpdateGameDetailsFilesTask);

                await statusController.UpdateProgressAsync(
                    enumerateUpdateGameDetailsFilesTask,
                    ++current,
                    updatedIdsCount,
                    gameDetails.Title);

                foreach (var gameDetailsFile in await itemizeGameDetailsFilesAsyncDelegate.ItemizeAsync(
                        gameDetails,
                        enumerateUpdateGameDetailsFilesTask))
                        yield return gameDetailsFile;
            }

            await statusController.CompleteAsync(enumerateUpdateGameDetailsFilesTask);
        }
    }
}
