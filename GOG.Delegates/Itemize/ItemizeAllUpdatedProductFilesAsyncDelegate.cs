using System.Collections.Generic;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Directory;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Itemize;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllUpdatedProductFilesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IDataController<long> updatedDataController;
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        readonly IDirectoryController directoryController;
        readonly IActionLogController actionLogController;

		[Dependencies(
			"Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.GameDetailsDataController,GOG.Controllers",
			"GOG.Delegates.Itemize.ItemizeGameDetailsDirectoriesAsyncDelegate,GOG.Delegates",
			"Controllers.Directory.DirectoryController,Controllers",
			"Controllers.Logs.ActionLogController,Controllers")]
        public ItemizeAllUpdatedProductFilesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IDirectoryController directoryController,
            IActionLogController actionLogController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
            this.directoryController = directoryController;
            this.actionLogController = actionLogController;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            actionLogController.StartAction("Enumerate updated productFiles");

            await foreach (var id in updatedDataController.ItemizeAllAsync())
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                actionLogController.IncrementActionProgress();

                var gameDetailsDirectories =
                    await itemizeGameDetailsDirectoriesAsyncDelegate.ItemizeAsync(
                        gameDetails);

                foreach (var gameDetailDirectory in gameDetailsDirectories)
                    foreach (var updatedFile in directoryController.EnumerateFiles(gameDetailDirectory))
                        yield return updatedFile;
            }

            actionLogController.CompleteAction();
        }
    }
}
