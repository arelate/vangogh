using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Itemize;

using Interfaces.Status;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeGameDetailsDirectoriesAsyncDelegate : IItemizeAsyncDelegate<GameDetails, string>
    {
        private IItemizeAsyncDelegate<GameDetails,string> itemizeGameDetailsManualUrlsAsyncDelegate;
        private IGetDirectoryAsyncDelegate getDirectoryDelegate;

        public ItemizeGameDetailsDirectoriesAsyncDelegate(
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate,
            IGetDirectoryAsyncDelegate getDirectoryDelegate)
        {
            this.itemizeGameDetailsManualUrlsAsyncDelegate = itemizeGameDetailsManualUrlsAsyncDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
        }

        public async Task<IEnumerable<string>> ItemizeAsync(GameDetails gameDetails, IStatus status)
        {
            var gameDetailsDirectories = new List<string>();

            foreach (var manualUrl in await itemizeGameDetailsManualUrlsAsyncDelegate.ItemizeAsync(gameDetails, status))
            {
                var directory = await getDirectoryDelegate.GetDirectoryAsync(manualUrl, status);

                if (!gameDetailsDirectories.Contains(directory))
                    gameDetailsDirectories.Add(directory);
            }

            return gameDetailsDirectories;
        }
    }
}
