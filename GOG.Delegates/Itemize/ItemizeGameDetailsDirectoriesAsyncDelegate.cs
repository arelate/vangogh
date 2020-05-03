using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Itemize;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeGameDetailsDirectoriesAsyncDelegate : IItemizeAsyncDelegate<GameDetails, string>
    {
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate;
        private readonly IGetValueDelegate<string,string> getDirectoryDelegate;

        [Dependencies(
            typeof(ItemizeGameDetailsManualUrlsAsyncDelegate),
            typeof(GetProductFilesDirectoryDelegate))]
        public ItemizeGameDetailsDirectoriesAsyncDelegate(
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate,
            IGetValueDelegate<string,string> getDirectoryDelegate)
        {
            this.itemizeGameDetailsManualUrlsAsyncDelegate = itemizeGameDetailsManualUrlsAsyncDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
        }

        public async Task<IEnumerable<string>> ItemizeAsync(GameDetails gameDetails)
        {
            var gameDetailsDirectories = new List<string>();

            foreach (var manualUrl in await itemizeGameDetailsManualUrlsAsyncDelegate.ItemizeAsync(gameDetails))
            {
                var directory = getDirectoryDelegate.GetValue(manualUrl);

                if (!gameDetailsDirectories.Contains(directory))
                    gameDetailsDirectories.Add(directory);
            }

            return gameDetailsDirectories;
        }
    }
}