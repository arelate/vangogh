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

namespace GOG.Delegates.EnumerateAll
{
    public class EnumerateAllUpdatedProductFilesAsyncDelegate : IEnumerateAllAsyncDelegate<string>
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateAsyncDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate;
        private IDirectoryController directoryController;
        private IStatusController statusController;

        public EnumerateAllUpdatedProductFilesAsyncDelegate(
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
            var enumerateUpdatedProductFilesTask = await statusController.CreateAsync(status, "Enumerate updated productFiles");

            var updatedIds = await updatedDataController.EnumerateIdsAsync(enumerateUpdatedProductFilesTask);
            var updatedIdsCount = await updatedDataController.CountAsync(enumerateUpdatedProductFilesTask);
            var current = 0;

            var updatedProductFiles = new List<string>();

            foreach (var id in updatedIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, enumerateUpdatedProductFilesTask);

                await statusController.UpdateProgressAsync(
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

            await statusController.CompleteAsync(enumerateUpdatedProductFilesTask);

            return updatedProductFiles;
        }
    }
}
