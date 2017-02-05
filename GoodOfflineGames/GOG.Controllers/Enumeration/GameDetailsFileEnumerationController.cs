using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Data;
using Interfaces.Destination;
using Interfaces.Routing;

using GOG.Models;

namespace GOG.Controllers.Enumeration
{
    public class GameDetailsFileEnumerationController : GameDetailsManualUrlEnumerationController
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IRoutingController routingController;

        public GameDetailsFileEnumerationController(
            string[] languages,
            string[] operatingSystems,
            IDataController<GameDetails> gameDetailsDataController,
            IRoutingController routingController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate) :
            base(
                languages,
                operatingSystems,
                gameDetailsDataController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
            this.routingController = routingController;
        }

        public override async Task<IList<string>> EnumerateAsync(long id)
        {
            var gameDetailsFiles = new List<string>();

            var gameDetailsManualUrls = await base.EnumerateAsync(id);
            var gameDetailsResolvedUris = await routingController.TraceRoutesAsync(id, gameDetailsManualUrls);

            // that means that routes information is incomplete and 
            // it's not possible to map manualUrls to resolvedUrls
            if (gameDetailsManualUrls.Count != gameDetailsResolvedUris.Count)
                return gameDetailsFiles;

            for (var ii = 0; ii < gameDetailsResolvedUris.Count; ii++)
            {
                // since we don't download all languages and operating systems 
                // we don't have routes for each and every gameDetails uri
                // however the ones we have should represent expected files for that product
                if (string.IsNullOrEmpty(gameDetailsResolvedUris[ii]))
                    continue;

                var localFileUri = Path.Combine(
                    getDirectoryDelegate.GetDirectory(gameDetailsManualUrls[ii]),
                    getFilenameDelegate.GetFilename(gameDetailsResolvedUris[ii]));

                gameDetailsFiles.Add(localFileUri);
            }

            return gameDetailsFiles;
        }
    }
}
