using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Status;
using Interfaces.Directory;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class UpdatedProductFilesEnumerateAllDelegate : IEnumerateAllAsyncDelegate<string>
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate;
        private IDirectoryController directoryController;
        private IStatusController statusController;

        public UpdatedProductFilesEnumerateAllDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateAsyncDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate,
            IDirectoryController directoryController,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsDirectoryEnumerateDelegate = gameDetailsDirectoryEnumerateDelegate;
            this.directoryController = directoryController;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> EnumerateAllAsync(IStatus status)
        {
            var enumerateUpdatedProductFilesTask = statusController.Create(status, "Enumerate updated productFiles");

            var updatedIds = await updatedDataController.EnumerateIdsAsync(enumerateUpdatedProductFilesTask);
            var updatedIdsCount = await updatedDataController.CountAsync(enumerateUpdatedProductFilesTask);
            var current = 0;

            var updatedProductFiles = new List<string>();

            foreach (var id in updatedIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, enumerateUpdatedProductFilesTask);

                statusController.UpdateProgress(
                    enumerateUpdatedProductFilesTask,
                    ++current,
                    updatedIdsCount,
                    gameDetails.Title);

                var gameDetailsDirectories = await gameDetailsDirectoryEnumerateDelegate.EnumerateAsync(gameDetails, enumerateUpdatedProductFilesTask);
                foreach (var gameDetailDirectory in gameDetailsDirectories)
                {
                    updatedProductFiles.AddRange(directoryController.EnumerateFiles(gameDetailDirectory));
                }
            }

            statusController.Complete(enumerateUpdatedProductFilesTask);

            return updatedProductFiles;
        }
    }
}
