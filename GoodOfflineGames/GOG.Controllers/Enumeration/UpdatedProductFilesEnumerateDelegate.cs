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
    public class UpdatedProductFilesEnumerateDelegate : IEnumerateAsyncDelegate
    {
        private IDataController<long> updatedDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate;
        private IDirectoryController directoryController;
        private IStatusController statusController;

        public UpdatedProductFilesEnumerateDelegate(
            IDataController<long> updatedDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<GameDetails> gameDetailsDirectoryEnumerateDelegate,
            IDirectoryController directoryController,
            IStatusController statusController)
        {
            this.updatedDataController = updatedDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.gameDetailsDirectoryEnumerateDelegate = gameDetailsDirectoryEnumerateDelegate;
            this.directoryController = directoryController;
            this.statusController = statusController;
        }

        public async Task<IList<string>> EnumerateAsync(IStatus status)
        {
            var enumerateUpdatedProductFilesTask = statusController.Create(status, "Enumerate updated productFiles");

            var updatedIds = updatedDataController.EnumerateIds();
            var updatedIdsCount = updatedDataController.Count();
            var current = 0;

            var updatedProductFiles = new List<string>();

            foreach (var id in updatedIds)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);

                statusController.UpdateProgress(
                    enumerateUpdatedProductFilesTask,
                    ++current,
                    updatedIdsCount,
                    gameDetails.Title);

                var gameDetailsDirectories = gameDetailsDirectoryEnumerateDelegate.Enumerate(gameDetails);
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
