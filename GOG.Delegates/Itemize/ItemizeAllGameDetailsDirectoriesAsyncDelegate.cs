using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Interfaces.Status;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllGameDetailsDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        readonly IStatusController statusController;

        public ItemizeAllGameDetailsDirectoriesAsyncDelegate(
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IStatusController statusController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
            this.statusController = statusController;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync(IStatus status)
        {
            var enumerateGameDetailsDirectoriesTask = await statusController.CreateAsync(status, "Enumerate gameDetails directories");
            
            var current = 0;
            var gameDetailsCount = await gameDetailsDataController.CountAsync(enumerateGameDetailsDirectoriesTask);

            await foreach (var gameDetails in gameDetailsDataController.ItemizeAllAsync(enumerateGameDetailsDirectoriesTask))
            {
                await statusController.UpdateProgressAsync(
                    enumerateGameDetailsDirectoriesTask,
                    ++current,
                    gameDetailsCount,
                    gameDetails.Title);

                foreach (var directory in await itemizeGameDetailsDirectoriesAsyncDelegate.ItemizeAsync(
                        gameDetails, 
                        enumerateGameDetailsDirectoriesTask))
                        yield return directory;
            }

            await statusController.CompleteAsync(enumerateGameDetailsDirectoriesTask);
        }
    }
}
