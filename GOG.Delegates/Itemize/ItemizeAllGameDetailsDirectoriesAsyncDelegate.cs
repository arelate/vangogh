using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAllGameDetailsDirectoriesAsyncDelegate : IItemizeAllAsyncDelegate<string>
    {
        private readonly IItemizeAllAsyncDelegate<GameDetails> itemizeAllGameDetailsAsyncDelegate;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsDirectoriesAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "GOG.Delegates.Itemize.ProductTypes.ItemizeAllGameDetailsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeGameDetailsDirectoriesAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
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