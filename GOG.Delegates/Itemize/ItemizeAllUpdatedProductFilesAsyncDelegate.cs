using System.Collections.Generic;
using System.IO;

using Interfaces.Controllers.Data;
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
        readonly IActionLogController actionLogController;

		[Dependencies(
			"Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.GameDetailsDataController,GOG.Controllers",
			"GOG.Delegates.Itemize.ItemizeGameDetailsDirectoriesAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]
        public ItemizeAllUpdatedProductFilesAsyncDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IActionLogController actionLogController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
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
                    foreach (var updatedFile in Directory.EnumerateFiles(gameDetailDirectory))
                        yield return updatedFile;
            }

            actionLogController.CompleteAction();
        }
    }
}
