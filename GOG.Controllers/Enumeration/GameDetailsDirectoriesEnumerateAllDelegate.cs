using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Data;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsDirectoriesEnumerateAllDelegate : IEnumerateAllAsyncDelegate<string>
    {
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate;
        private IStatusController statusController;

        public GameDetailsDirectoriesEnumerateAllDelegate(
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateAsyncDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate,
            IStatusController statusController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsDirectoryEnumerateDelegate = gameDetailsDirectoryEnumerateDelegate;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> EnumerateAllAsync(IStatus status)
        {
            var enumerateGameDetailsDirectoriesTask = await statusController.CreateAsync(status, "Enumerate gameDetails directories");
            var directories = new List<string>();
            var current = 0;
            var gameDetailsIds = await gameDetailsDataController.EnumerateIdsAsync(enumerateGameDetailsDirectoriesTask);
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
                    await gameDetailsDirectoryEnumerateDelegate.EnumerateAsync(
                        gameDetails, 
                        enumerateGameDetailsDirectoriesTask));
            }

            await statusController.CompleteAsync(enumerateGameDetailsDirectoriesTask);

            return directories;
        }
    }
}
