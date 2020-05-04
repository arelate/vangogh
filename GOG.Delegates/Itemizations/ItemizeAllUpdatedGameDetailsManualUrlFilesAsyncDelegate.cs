using System.Collections.Generic;
using Attributes;
using Delegates.Activities;
using Delegates.Itemizations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private readonly IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate;
        private readonly IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsFilesAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate),
            typeof(ItemizeGameDetailsFilesAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsFilesAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllUpdatedAsyncDelegate = itemizeAllUpdatedAsyncDelegate;
            this.getGameDetailsByIdAsyncDelegate = getGameDetailsByIdAsyncDelegate;
            this.itemizeGameDetailsFilesAsyncDelegate = itemizeGameDetailsFilesAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            startDelegate.Start("Enumerate updated gameDetails files");

            await foreach (var id in itemizeAllUpdatedAsyncDelegate.ItemizeAllAsync())
            {
                var gameDetails = await getGameDetailsByIdAsyncDelegate.GetDataAsync(id);

                setProgressDelegate.SetProgress();

                foreach (var gameDetailsFile in await itemizeGameDetailsFilesAsyncDelegate.ItemizeAsync(
                    gameDetails))
                    yield return gameDetailsFile;
            }

            completeDelegate.Complete();
        }
    }
}