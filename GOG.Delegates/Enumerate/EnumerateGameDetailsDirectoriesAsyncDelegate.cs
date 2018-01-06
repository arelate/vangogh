using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Settings;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Enumeration;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Delegates.Enumerate
{
    public class EnumerateGameDetailsDirectoriesAsyncDelegate : IEnumerateAsyncDelegate<GameDetails>
    {
        private IEnumerateAsyncDelegate<GameDetails> manualUrlEnumerateDelegate;
        private IGetDirectoryDelegate getDirectoryDelegate;

        public EnumerateGameDetailsDirectoriesAsyncDelegate(
            IEnumerateAsyncDelegate<GameDetails> manualUrlEnumerateDelegate,
            IGetDirectoryDelegate getDirectoryDelegate)
        {
            this.manualUrlEnumerateDelegate = manualUrlEnumerateDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
        }

        public async Task<IEnumerable<string>> EnumerateAsync(GameDetails gameDetails, IStatus status)
        {
            var gameDetailsDirectories = new List<string>();

            foreach (var manualUrl in await manualUrlEnumerateDelegate.EnumerateAsync(gameDetails, status))
            {
                var directory = getDirectoryDelegate.GetDirectory(manualUrl);

                if (!gameDetailsDirectories.Contains(directory))
                    gameDetailsDirectories.Add(directory);
            }

            return gameDetailsDirectories;
        }
    }
}
