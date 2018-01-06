using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Routing;
using Interfaces.Enumeration;
using Interfaces.Status;

using GOG.Models;
using System;

namespace GOG.Delegates.Enumerate
{
    public class EnumerateGameDetailsFilesAsyncDelegate : IEnumerateAsyncDelegate<GameDetails>
    {
        private IEnumerateAsyncDelegate<GameDetails> manualUrlEnumerationDelegate;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IRoutingController routingController;
        private IStatusController statusController;

        public EnumerateGameDetailsFilesAsyncDelegate(
            IEnumerateAsyncDelegate<GameDetails> manualUrlEnumerationDelegate,
            IRoutingController routingController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IStatusController statusController)
        {
            this.manualUrlEnumerationDelegate = manualUrlEnumerationDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
            this.routingController = routingController;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> EnumerateAsync(GameDetails gameDetails, IStatus status)
        {
            var enumerateGameDetailsFilesStatus = await statusController.CreateAsync(status, "Enumerate game details files");

            var gameDetailsFiles = new List<string>();

            var gameDetailsManualUrls = await manualUrlEnumerationDelegate.EnumerateAsync(gameDetails, status);
            var gameDetailsManualUrlsCount = gameDetailsManualUrls.Count();
            var gameDetailsResolvedUris = await routingController.TraceRoutesAsync(
                gameDetails.Id, 
                gameDetailsManualUrls, 
                enumerateGameDetailsFilesStatus);

            // that means that routes information is incomplete and 
            // it's not possible to map manualUrls to resolvedUrls
            if (gameDetailsManualUrlsCount != gameDetailsResolvedUris.Count)
            {
                await statusController.CompleteAsync(enumerateGameDetailsFilesStatus);
                throw new ArgumentException($"Product {gameDetails.Id} resolvedUris count doesn't match manualUrls count");
            }

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

            await statusController.CompleteAsync(enumerateGameDetailsFilesStatus);

            if (gameDetailsManualUrlsCount != gameDetailsFiles.Count)
                throw new ArgumentException($"Product {gameDetails.Id} files count doesn't match manualUrls count");

            return gameDetailsFiles;
        }
    }
}
