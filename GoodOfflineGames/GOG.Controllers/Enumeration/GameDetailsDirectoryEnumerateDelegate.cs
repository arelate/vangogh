using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Settings;
using Interfaces.Destination.Directory;
using Interfaces.Enumeration;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsDirectoryEnumerateDelegate: IEnumerateDelegate<GameDetails>
    {
        private IEnumerateDelegate<GameDetails> manualUrlEnumerateDelegate;
        private IGetDirectoryDelegate getDirectoryDelegate;

        public GameDetailsDirectoryEnumerateDelegate(
            IEnumerateDelegate<GameDetails> manualUrlEnumerateDelegate,
            IGetDirectoryDelegate getDirectoryDelegate)
        {
            this.manualUrlEnumerateDelegate = manualUrlEnumerateDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
        }

        public IEnumerable<string> Enumerate(GameDetails gameDetails)
        {
            var gameDetailsDirectories = new List<string>();

            foreach (var manualUrl in manualUrlEnumerateDelegate.Enumerate(gameDetails))
            {
                var directory = getDirectoryDelegate.GetDirectory(manualUrl);

                if (!gameDetailsDirectories.Contains(directory))
                {
                    gameDetailsDirectories.Add(directory);
                    yield return directory;
                }
            }
        }
    }
}
