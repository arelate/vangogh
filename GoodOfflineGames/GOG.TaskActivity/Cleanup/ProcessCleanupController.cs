using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Reporting;
using Interfaces.Routing;
using Interfaces.Destination;

using Models.Uris;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Cleanup
{
    public class ProcessCleanupController: TaskActivityController
    {
        private IDataController<long> scheduledCleanupDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IRoutingController routingController;
        private IDestinationController destinationController;

        public ProcessCleanupController(
            IDataController<long> scheduledCleanupDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IRoutingController routingController,
            IDestinationController destinationController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.scheduledCleanupDataController = scheduledCleanupDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.routingController = routingController;
            this.destinationController = destinationController;
        }

        public async override Task ProcessTaskAsync()
        {
            foreach (var id in scheduledCleanupDataController.EnumerateIds())
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);
                if (gameDetails == null)
                {
                    taskReportingController.ReportWarning(
                        string.Format(
                            "Cleanup scheduled for account product that doesn't exist: {0}", 
                            id));
                    continue;
                }

                var gameDetailsDownloadEntries = new List<DownloadEntry>();

                foreach (var download in gameDetails.LanguageDownloads)
                {
                    if (download.Windows != null) gameDetailsDownloadEntries.AddRange(download.Windows);
                    if (download.Mac != null) gameDetailsDownloadEntries.AddRange(download.Mac);
                    if (download.Linux != null) gameDetailsDownloadEntries.AddRange(download.Linux);
                }

                if (gameDetails.Extras != null) gameDetailsDownloadEntries.AddRange(gameDetails.Extras);

                var gameDetailsSources = new List<string>();

                foreach (var downloadEntry in gameDetailsDownloadEntries)
                {
                    // use same absolute uri expansion we use in ProductDownloadSourcesController
                    var absoluteSourceUri = string.Format(Uris.Paths.ProductFiles.FullUriTemplate, downloadEntry.ManualUrl);
                    gameDetailsSources.Add(absoluteSourceUri);
                }

                var gameDetailsUris = await routingController.TraceRoutesAsync(id, gameDetailsSources);

                var filesWhitelist = new List<string>();

                for (var ii=0; ii<gameDetailsUris.Count; ii++)
                {
                    // since we don't download all languages and operating systems 
                    // we don't have routes for each and every gameDetails uri
                    // however the ones we have should represent expected files for that product
                    if (string.IsNullOrEmpty(gameDetailsUris[ii]))
                        continue;

                    // local file directory comes from ManualUrl and filename from resolvedUri
                    var localFileUri = Path.Combine(
                        destinationController.GetDirectory(gameDetailsSources[ii]),
                        destinationController.GetFilename(gameDetailsUris[ii]));

                    filesWhitelist.Add(localFileUri);
                }

                taskReportingController.ReportWarning("Cleaning up product " + gameDetails.Title);
            }
        }
    }
}
