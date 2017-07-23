using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.FileDownload;
using Interfaces.Data;
using Interfaces.Status;
using Interfaces.ContextDefinitions;

using Models.ProductDownloads;
using Models.Separators;

namespace GOG.Activities.Download
{
    public class DownloadActivity : Activity
    {
        private Context context;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDownloadFileFromSourceAsyncDelegate downloadFileFromSourceDelegate;

        public DownloadActivity(
            Context context,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadFileFromSourceAsyncDelegate downloadFileFromSourceDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.context = context;
            this.productDownloadsDataController = productDownloadsDataController;
            this.downloadFileFromSourceDelegate = downloadFileFromSourceDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            var current = 0;
            var productDownloadsData = productDownloadsDataController.EnumerateIds();
            var total = productDownloadsDataController.Count();

            var processDownloadsTask = statusController.Create(status, 
                $"Process updated {context} downloads");

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
                    d.Context == context).ToArray();

                var processDownloadEntriesTask = statusController.Create(processDownloadsTask, 
                    $"Download {context} entries");

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
                        $"Remove scheduled {context} downloaded entry");

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
