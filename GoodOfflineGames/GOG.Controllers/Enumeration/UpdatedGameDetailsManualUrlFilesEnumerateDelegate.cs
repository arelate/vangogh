using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class UpdatedGameDetailsManualUrlFilesEnumerateDelegate : IEnumerateAsyncDelegate
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsFilesEnumerateDelegate;
        private ITaskStatusController taskStatusController;

        public UpdatedGameDetailsManualUrlFilesEnumerateDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateAsyncDelegate<GameDetails> gameDetailsFilesEnumerateDelegate,
            ITaskStatusController taskStatusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsFilesEnumerateDelegate = gameDetailsFilesEnumerateDelegate;
            this.taskStatusController = taskStatusController;
        }

        public async Task<IList<string>> EnumerateAsync(ITaskStatus taskStatus)
        {
            var enumerateUpdateGameDetailsFilesTask = taskStatusController.Create(taskStatus, "Enumerate updated gameDetails files");

            var updatedIds = updatedDataController.EnumerateIds();
            var updatedIdsCount = updatedDataController.Count();
            var current = 0;

            var gameDetailsFiles = new List<string>();

            foreach (var id in updatedIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                taskStatusController.UpdateProgress(
                    enumerateUpdateGameDetailsFilesTask,
                    ++current,
                    updatedIdsCount,
                    gameDetails.Title);

                gameDetailsFiles.AddRange(await gameDetailsFilesEnumerateDelegate.EnumerateAsync(gameDetails));
            }

            taskStatusController.Complete(enumerateUpdateGameDetailsFilesTask);

            return gameDetailsFiles;
        }
    }
}
