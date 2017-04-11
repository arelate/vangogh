using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Routing;
using Interfaces.Enumeration;

using GOG.Models;
using System;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsFileEnumerateDelegate : IEnumerateAsyncDelegate<GameDetails>
    {
        private IEnumerateDelegate<GameDetails> manualUrlEnumerationDelegate;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IRoutingController routingController;

        public GameDetailsFileEnumerateDelegate(
            IEnumerateDelegate<GameDetails> manualUrlEnumerationDelegate,
            IRoutingController routingController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate)
        {
            this.manualUrlEnumerationDelegate = manualUrlEnumerationDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
            this.routingController = routingController;
        }

        public async Task<IList<string>> EnumerateAsync(GameDetails gameDetails)
        {
            var gameDetailsFiles = new List<string>();

            var gameDetailsManualUrls = manualUrlEnumerationDelegate.Enumerate(gameDetails);
            var gameDetailsManualUrlsCount = gameDetailsManualUrls.Count();
            var gameDetailsResolvedUris = await routingController.TraceRoutesAsync(gameDetails.Id, gameDetailsManualUrls);

            // that means that routes information is incomplete and 
            // it's not possible to map manualUrls to resolvedUrls
            if (gameDetailsManualUrlsCount != gameDetailsResolvedUris.Count)
                throw new ArgumentException($"Product {gameDetails.Id} resolvedUris count doesn't match manualUrls count");

            for (var ii = 0; ii < gameDetailsResolvedUris.Count; ii++)
            {
                // since we don't download all languages and operating systems 
                // we don't have routes for each and every gameDetails uri
                // however the ones we have should represent expected files for that product
                if (string.IsNullOrEmpty(gameDetailsResolvedUris[ii]))
                    continue;

                var localFileUri = Path.Combine(
                    getDirectoryDelegate.GetDirectory(gameDetailsManualUrls.ElementAt(ii)),
                    getFilenameDelegate.GetFilename(gameDetailsResolvedUris[ii]));

                gameDetailsFiles.Add(localFileUri);
            }

            if (gameDetailsManualUrlsCount != gameDetailsFiles.Count)
                throw new ArgumentException($"Product {gameDetails.Id} files count doesn't match manualUrls count");

            return gameDetailsFiles;
        }
    }
}
