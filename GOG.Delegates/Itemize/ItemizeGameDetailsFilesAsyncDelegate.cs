using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.GetPath;
using Interfaces.Delegates.Itemize;

using Interfaces.Routing;
using Interfaces.Status;

using GOG.Models;
using System;

namespace GOG.Delegates.Itemize
{
    public class ItemizeGameDetailsFilesAsyncDelegate : IItemizeAsyncDelegate<GameDetails, string>
    {
        private IItemizeAsyncDelegate<GameDetails,string> itemizeGameDetailsManualUrlsDelegate;
        private IGetPathDelegate getPathDelegate;
        //private IGetDirectoryDelegate getDirectoryDelegate;
        //private IGetFilenameDelegate getFilenameDelegate;
        private IRoutingController routingController;
        private IStatusController statusController;

        public ItemizeGameDetailsFilesAsyncDelegate(
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsDelegate,
            IRoutingController routingController,
            IGetPathDelegate getPathDelegate,
            //IGetDirectoryDelegate getDirectoryDelegate,
            //IGetFilenameDelegate getFilenameDelegate,
            IStatusController statusController)
        {
            this.itemizeGameDetailsManualUrlsDelegate = itemizeGameDetailsManualUrlsDelegate;
            //this.getDirectoryDelegate = getDirectoryDelegate;
            //this.getFilenameDelegate = getFilenameDelegate;
            this.getPathDelegate = getPathDelegate;
            this.routingController = routingController;
            this.statusController = statusController;
        }

        public async Task<IEnumerable<string>> ItemizeAsync(GameDetails gameDetails, IStatus status)
        {
            var enumerateGameDetailsFilesStatus = await statusController.CreateAsync(status, "Enumerate game details files");

            var gameDetailsFiles = new List<string>();

            var gameDetailsManualUrls = await itemizeGameDetailsManualUrlsDelegate.ItemizeAsync(gameDetails, status);
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

                //var localFileUri = Path.Combine(
                //    getDirectoryDelegate.GetDirectory(gameDetailsManualUrls.ElementAt(ii)),
                //getFilenameDelegate.GetFilename(gameDetailsResolvedUris[ii]));

                var localFilePath = getPathDelegate.GetPath(
                    gameDetailsManualUrls.ElementAt(ii),
                    gameDetailsResolvedUris[ii]);

                gameDetailsFiles.Add(localFilePath);
            }

            await statusController.CompleteAsync(enumerateGameDetailsFilesStatus);

            if (gameDetailsManualUrlsCount != gameDetailsFiles.Count)
                throw new ArgumentException($"Product {gameDetails.Id} files count doesn't match manualUrls count");

            return gameDetailsFiles;
        }
    }
}
