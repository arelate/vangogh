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
        private IDataController<long, GameDetails> gameDetailsDataController;
        private IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        private IStatusController statusController;

        public ItemizeAllGameDetailsDirectoriesAsyncDelegate(
            IDataController<long, GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IStatusController statusController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> ItemizeAllAsync(IStatus status)
        {
            var enumerateGameDetailsDirectoriesTask = await statusController.CreateAsync(status, "Enumerate gameDetails directories");
            var directories = new List<string>();
            var current = 0;
            var gameDetailsIds = await gameDetailsDataController.ItemizeAllAsync(enumerateGameDetailsDirectoriesTask);
            var gameDetailsCount = await gameDetailsDataController.CountAsync(enumerateGameDetailsDirectoriesTask);

            foreach (var id in gameDetailsIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, enumerateGameDetailsDirectoriesTask);

                await statusController.UpdateProgressAsync(
                    enumerateGameDetailsDirectoriesTask,
                    ++current,
                    gameDetailsCount,
                    gameDetails.Title);

                directories.AddRange(
                    await itemizeGameDetailsDirectoriesAsyncDelegate.ItemizeAsync(
                        gameDetails, 
                        enumerateGameDetailsDirectoriesTask));
            }

            await statusController.CompleteAsync(enumerateGameDetailsDirectoriesTask);

            return directories;
        }
    }
}
