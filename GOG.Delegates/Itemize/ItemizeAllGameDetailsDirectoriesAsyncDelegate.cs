using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllGameDetailsDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        readonly IActionLogController actionLogController;

		[Dependencies(
			"GOG.Controllers.Data.ProductTypes.GameDetailsDataController,GOG.Controllers",
			"GOG.Delegates.Itemize.ItemizeGameDetailsDirectoriesAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]
        public ItemizeAllGameDetailsDirectoriesAsyncDelegate(
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IActionLogController actionLogController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
            this.actionLogController = actionLogController;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            actionLogController.StartAction("Enumerate gameDetails directories");
            
            await foreach (var gameDetails in gameDetailsDataController.ItemizeAllAsync())
            {
                actionLogController.IncrementActionProgress();

                foreach (var directory in await itemizeGameDetailsDirectoriesAsyncDelegate.ItemizeAsync(
                        gameDetails))
                        yield return directory;
            }

            actionLogController.CompleteAction();
        }
    }
}
