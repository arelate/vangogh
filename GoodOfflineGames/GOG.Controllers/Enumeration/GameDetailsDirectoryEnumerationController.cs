using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Destination.Directory;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsDirectoryEnumerationController: GameDetailsManualUrlEnumerationController
    {
        private IGetDirectoryDelegate getDirectoryDelegate;

        public GameDetailsDirectoryEnumerationController(
            string[] languages,
            string[] operatingSystems,
            IDataController<GameDetails> gameDetailsDataController,
            IGetDirectoryDelegate getDirectoryDelegate):
            base(
                languages,
                operatingSystems,
                gameDetailsDataController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
        }

        public override async Task<IList<string>> EnumerateAsync(long id)
        {
            var gameDetailsDirectories = new List<string>();

            var gameDetailsManualUrls = await base.EnumerateAsync(id);

            foreach (var manualUrl in gameDetailsManualUrls)
            {
                var directory = getDirectoryDelegate.GetDirectory(manualUrl);

                if (!gameDetailsDirectories.Contains(directory))
                    gameDetailsDirectories.Add(directory);
            }

            return gameDetailsDirectories;
        }
    }
}
