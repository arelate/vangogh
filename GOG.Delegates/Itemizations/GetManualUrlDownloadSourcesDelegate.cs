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
    public class ItemizeAllManualUrlDownloadSourcesAsyncDelegate : 
        IItemizeAllAsyncDelegate<(long, IList<string>)>
    {
        private readonly IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate;
        private readonly IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncController;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate),
            typeof(ItemizeGameDetailsManualUrlsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllManualUrlDownloadSourcesAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncController,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllUpdatedAsyncDelegate = itemizeAllUpdatedAsyncDelegate;
            this.getGameDetailsByIdAsyncDelegate = getGameDetailsByIdAsyncDelegate;
            this.itemizeGameDetailsManualUrlsAsyncController = itemizeGameDetailsManualUrlsAsyncController;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async IAsyncEnumerable<(long, IList<string>)> ItemizeAllAsync()
        {
            startDelegate.Start("Get download sources");

            await foreach (var id in itemizeAllUpdatedAsyncDelegate.ItemizeAllAsync())
            {
                setProgressDelegate.SetProgress();

                var gameDetails = await getGameDetailsByIdAsyncDelegate.GetDataAsync(id);

                yield return (
                    id, 
                    new List<string>(
                        await itemizeGameDetailsManualUrlsAsyncController.ItemizeAsync(gameDetails)));
            }

            completeDelegate.Complete();
        }
    }
}