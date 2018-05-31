using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Directory;

using Interfaces.Delegates.Itemize;

using Interfaces.Status;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllUpdatedProductFilesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IIndexController<long> updatedDataController;
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        readonly IDirectoryController directoryController;
        readonly IStatusController statusController;

        public ItemizeAllUpdatedProductFilesAsyncDelegate(
            IIndexController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IDirectoryController directoryController,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
            this.directoryController = directoryController;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> ItemizeAllAsync(IStatus status)
        {
            var enumerateUpdatedProductFilesTask = await statusController.CreateAsync(status, "Enumerate updated productFiles");

            var updatedIds = await updatedDataController.ItemizeAllAsync(enumerateUpdatedProductFilesTask);
            var updatedIdsCount = await updatedDataController.CountAsync(enumerateUpdatedProductFilesTask);
            var current = 0;

            var updatedProductFiles = new List<string>();

            foreach (var id in updatedIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, enumerateUpdatedProductFilesTask);

                await statusController.UpdateProgressAsync(
                    enumerateUpdatedProductFilesTask,
                    ++current,
                    updatedIdsCount,
                    gameDetails.Title);

                var gameDetailsDirectories = 
                    await itemizeGameDetailsDirectoriesAsyncDelegate.ItemizeAsync(
                        gameDetails, 
                        enumerateUpdatedProductFilesTask);

                foreach (var gameDetailDirectory in gameDetailsDirectories)
                {
                    updatedProductFiles.AddRange(directoryController.EnumerateFiles(gameDetailDirectory));
                }
            }

            await statusController.CompleteAsync(enumerateUpdatedProductFilesTask);

            return updatedProductFiles;
        }
    }
}
