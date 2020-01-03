using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.GetPath;
using Interfaces.Delegates.Itemize;
using Interfaces.Models.Dependencies;

using Interfaces.Controllers.Logs;

using Interfaces.Routing;


using Attributes;

using GOG.Models;
using System;

namespace GOG.Delegates.Itemize
{
    public class ItemizeGameDetailsFilesAsyncDelegate : IItemizeAsyncDelegate<GameDetails, string>
    {
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsDelegate;
        readonly IGetPathDelegate getPathDelegate;
        //private IGetDirectoryDelegate getDirectoryDelegate;
        //private IGetFilenameDelegate getFilenameDelegate;
        readonly IRoutingController routingController;
        readonly IActionLogController actionLogController;

		[Dependencies(
            DependencyContext.Default,
			"GOG.Delegates.Itemize.ItemizeGameDetailsManualUrlsAsyncDelegate,GOG.Delegates",
			"Controllers.Routing.RoutingController,Controllers",
			"Delegates.GetPath.Json.GetGameDetailsFilesPathDelegate,Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]
        public ItemizeGameDetailsFilesAsyncDelegate(
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsDelegate,
            IRoutingController routingController,
            IGetPathDelegate getPathDelegate,
            //IGetDirectoryDelegate getDirectoryDelegate,
            //IGetFilenameDelegate getFilenameDelegate,
            IActionLogController actionLogController)
        {
            this.itemizeGameDetailsManualUrlsDelegate = itemizeGameDetailsManualUrlsDelegate;
            //this.getDirectoryDelegate = getDirectoryDelegate;
            //this.getFilenameDelegate = getFilenameDelegate;
            this.getPathDelegate = getPathDelegate;
            this.routingController = routingController;
            this.actionLogController = actionLogController;
        }

        public async Task<IEnumerable<string>> ItemizeAsync(GameDetails gameDetails)
        {
            actionLogController.StartAction("Enumerate game details files");

            var gameDetailsFiles = new List<string>();

            var gameDetailsManualUrls = await itemizeGameDetailsManualUrlsDelegate.ItemizeAsync(gameDetails);
            var gameDetailsManualUrlsCount = gameDetailsManualUrls.Count();
            var gameDetailsResolvedUris = await routingController.TraceRoutesAsync(
                gameDetails.Id, 
                gameDetailsManualUrls);

            // that means that routes information is incomplete and 
            // it's not possible to map manualUrls to resolvedUrls
            if (gameDetailsManualUrlsCount != gameDetailsResolvedUris.Count)
            {
                actionLogController.CompleteAction();
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

            actionLogController.CompleteAction();

            if (gameDetailsManualUrlsCount != gameDetailsFiles.Count)
                throw new ArgumentException($"Product {gameDetails.Id} files count doesn't match manualUrls count");

            return gameDetailsFiles;
        }
    }
}
