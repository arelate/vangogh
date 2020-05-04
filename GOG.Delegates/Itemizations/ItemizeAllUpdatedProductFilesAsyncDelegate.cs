using System.Collections.Generic;
using System.IO;
using Attributes;
using Delegates.Activities;
using Delegates.Itemizations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeAllUpdatedProductFilesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private readonly IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate;
        private readonly IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate),
            typeof(ItemizeGameDetailsDirectoriesAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllUpdatedProductFilesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllUpdatedAsyncDelegate = itemizeAllUpdatedAsyncDelegate;
            this.getGameDetailsByIdAsyncDelegate = getGameDetailsByIdAsyncDelegate;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            startDelegate.Start("Enumerate updated productFiles");

            await foreach (var id in itemizeAllUpdatedAsyncDelegate.ItemizeAllAsync())
            {
                var gameDetails = await getGameDetailsByIdAsyncDelegate.GetDataAsync(id);

                setProgressDelegate.SetProgress();

                var gameDetailsDirectories =
                    await itemizeGameDetailsDirectoriesAsyncDelegate.ItemizeAsync(
                        gameDetails);

                foreach (var gameDetailDirectory in gameDetailsDirectories)
                foreach (var updatedFile in Directory.EnumerateFiles(gameDetailDirectory))
                    yield return updatedFile;
            }

            completeDelegate.Complete();
        }
    }
}