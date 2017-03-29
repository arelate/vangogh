using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.FileDownload;
using Interfaces.Data;
using Interfaces.Status;

using Models.ProductDownloads;
using Models.Separators;

namespace GOG.Activities.Download
{
    public class DownloadActivity : Activity
    {
        private string downloadParameter;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDownloadFileFromSourceDelegate downloadFileFromSourceDelegate;

        public DownloadActivity(
            string downloadParameter,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadFileFromSourceDelegate downloadFileFromSourceDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.downloadParameter = downloadParameter;
            this.productDownloadsDataController = productDownloadsDataController;
            this.downloadFileFromSourceDelegate = downloadFileFromSourceDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var current = 0;
            var productDownloadsData = productDownloadsDataController.EnumerateIds();
            var total = productDownloadsDataController.Count();

            var processDownloadsTask = statusController.Create(status, 
                $"Process updated {downloadParameter} downloads");

            var emptyProductDownloads = new List<ProductDownloads>();

            foreach (var id in productDownloadsData)
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                statusController.UpdateProgress(
                    processDownloadsTask,
                    ++current,
                    total,
                    productDownloads.Title);

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.FindAll(
                    d => 
                    d.DownloadParameter == downloadParameter).ToArray();

                var processDownloadEntriesTask = statusController.Create(processDownloadsTask, 
                    $"Download {downloadParameter} entries");

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    var sanitizedUri = entry.SourceUri;
                    if (sanitizedUri.Contains(Separators.QueryString))
                        sanitizedUri = sanitizedUri.Substring(0, sanitizedUri.IndexOf(Separators.QueryString));

                    statusController.UpdateProgress(
                        processDownloadEntriesTask,
                        ii + 1,
                        downloadEntries.Length,
                        sanitizedUri);

                    await downloadFileFromSourceDelegate.DownloadFileFromSourceAsync(
                        id,
                        productDownloads.Title,
                        sanitizedUri,
                        entry.Destination,
                        processDownloadEntriesTask);

                    var removeEntryTask = statusController.Create(
                        processDownloadEntriesTask,
                        $"Remove scheduled {downloadParameter} downloaded entry");

                    productDownloads.Downloads.Remove(entry);
                    await productDownloadsDataController.UpdateAsync(removeEntryTask, productDownloads);

                    statusController.Complete(removeEntryTask);
                }

                // if there are no scheduled downloads left - mark file for removal
                if (productDownloads.Downloads.Count == 0)
                    emptyProductDownloads.Add(productDownloads);

                statusController.Complete(processDownloadEntriesTask);
            }

            var clearEmptyDownloadsTask = statusController.Create(processDownloadsTask, "Clear empty downloads");
            await productDownloadsDataController.RemoveAsync(clearEmptyDownloadsTask, emptyProductDownloads.ToArray());
            statusController.Complete(clearEmptyDownloadsTask);

            statusController.Complete(processDownloadsTask);
        }
    }
}
