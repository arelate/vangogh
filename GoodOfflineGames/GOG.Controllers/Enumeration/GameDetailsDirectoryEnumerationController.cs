using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Destination;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsDirectoryEnumerationController: GameDetailsManualUrlEnumerationController
    {
        private IDestinationController destinationController;

        public GameDetailsDirectoryEnumerationController(
            string[] languages,
            string[] operatingSystems,
            IDataController<GameDetails> gameDetailsDataController,
            IDestinationController destinationController):
            base(
                languages,
                operatingSystems,
                gameDetailsDataController)
        {
            this.destinationController = destinationController;
        }

        public override async Task<IList<string>> EnumerateAsync(long id)
        {
            var gameDetailsDirectories = new List<string>();

            var gameDetailsManualUrls = await base.EnumerateAsync(id);

            foreach (var manualUrl in gameDetailsManualUrls)
            {
                var directory = destinationController.GetDirectory(manualUrl);

                if (!gameDetailsDirectories.Contains(directory))
                    gameDetailsDirectories.Add(directory);
            }

            return gameDetailsDirectories;
        }
    }
}
