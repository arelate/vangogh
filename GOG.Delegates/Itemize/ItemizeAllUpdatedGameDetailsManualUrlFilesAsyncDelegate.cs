using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Itemize
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
            "Delegates.Itemize.ProductTypes.ItemizeAllUpdatedAsyncDelegate,Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeGameDetailsFilesAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
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