using System.Collections.Generic;
using Attributes;
using Delegates.Activities;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeAllGameDetailsDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private readonly IItemizeAllAsyncDelegate<GameDetails> itemizeAllGameDetailsAsyncDelegate;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ProductTypes.ItemizeAllGameDetailsAsyncDelegate),
            typeof(ItemizeGameDetailsDirectoriesAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public ItemizeAllGameDetailsDirectoriesAsyncDelegate(
            IItemizeAllAsyncDelegate<GameDetails> itemizeAllGameDetailsAsyncDelegate,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllGameDetailsAsyncDelegate = itemizeAllGameDetailsAsyncDelegate;
            this.itemizeGameDetailsDirectoriesAsyncDelegate = itemizeGameDetailsDirectoriesAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async IAsyncEnumerable<string> ItemizeAllAsync()
        {
            startDelegate.Start("Enumerate gameDetails directories");

            await foreach (var gameDetails in itemizeAllGameDetailsAsyncDelegate.ItemizeAllAsync())
            {
                setProgressDelegate.SetProgress();

                foreach (var directory in await itemizeGameDetailsDirectoriesAsyncDelegate.ItemizeAsync(
                    gameDetails))
                    yield return directory;
            }

            completeDelegate.Complete();
        }
    }
}