using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsDirectoriesEnumerateDelegate : IEnumerateAsyncDelegate
    {
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate;
        private ITaskStatusController taskStatusController;

        public GameDetailsDirectoriesEnumerateDelegate(
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate,
            ITaskStatusController taskStatusController)
        {
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsDirectoryEnumerateDelegate = gameDetailsDirectoryEnumerateDelegate;
            this.taskStatusController = taskStatusController;
        }

        public async Task<IList<string>> EnumerateAsync(ITaskStatus taskStatus)
        {
            var enumerateGameDetailsDirectoriesTask = taskStatusController.Create(taskStatus, "Enumerate gameDetails directories");
            var directories = new List<string>();
            var current = 0;
            var gameDetailsIds = gameDetailsDataController.EnumerateIds();
            var gameDetailsCount = gameDetailsDataController.Count();

            foreach (var id in gameDetailsIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                taskStatusController.UpdateProgress(
                    enumerateGameDetailsDirectoriesTask,
                    ++current,
                    gameDetailsCount,
                    gameDetails.Title);

                directories.AddRange(gameDetailsDirectoryEnumerateDelegate.Enumerate(gameDetails));
            }

            taskStatusController.Complete(enumerateGameDetailsDirectoriesTask);

            return directories;
        }
    }
}
