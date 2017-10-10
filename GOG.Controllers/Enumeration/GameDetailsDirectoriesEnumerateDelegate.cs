using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Data;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsDirectoriesEnumerateAllDelegate : IEnumerateAllAsyncDelegate
    {
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate;
        private IStatusController statusController;

        public GameDetailsDirectoriesEnumerateAllDelegate(
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate,
            IStatusController statusController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsDirectoryEnumerateDelegate = gameDetailsDirectoryEnumerateDelegate;
            this.statusController = statusController;
        }

        public async Task<IList<string>> EnumerateAllAsync(IStatus status)
        {
            var enumerateGameDetailsDirectoriesTask = statusController.Create(status, "Enumerate gameDetails directories");
            var directories = new List<string>();
            var current = 0;
            var gameDetailsIds = gameDetailsDataController.EnumerateIds();
            var gameDetailsCount = gameDetailsDataController.Count();

            foreach (var id in gameDetailsIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                statusController.UpdateProgress(
                    enumerateGameDetailsDirectoriesTask,
                    ++current,
                    gameDetailsCount,
                    gameDetails.Title);

                directories.AddRange(gameDetailsDirectoryEnumerateDelegate.Enumerate(gameDetails));
            }

            statusController.Complete(enumerateGameDetailsDirectoriesTask);

            return directories;
        }
    }
}
