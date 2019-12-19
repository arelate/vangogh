using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Interfaces.Activity;

using Models.ProductTypes;
using Models.Separators;

using GOG.Interfaces.Delegates.DownloadProductFile;

namespace GOG.Activities.DownloadProductFiles
{
    public abstract class DownloadFilesActivity<Type>: IActivity
        where Type:ProductCore
    {
        readonly IDataController<ProductDownloads> productDownloadsDataController;
        readonly IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate;
        readonly IActionLogController actionLogController;

        public DownloadFilesActivity(
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IActionLogController actionLogController)
        {
            this.productDownloadsDataController = productDownloadsDataController;
            this.downloadProductFileAsyncDelegate = downloadProductFileAsyncDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task ProcessActivityAsync()
        {
            actionLogController.StartAction(
                $"Process updated {typeof(Type)} downloads");

            var emptyProductDownloads = new List<ProductDownloads>();

            await foreach (var productDownloads in productDownloadsDataController.ItemizeAllAsync())
            {
                if (productDownloads == null) continue;

                // await statusController.UpdateProgressAsync(
                //     processDownloadsTask,
                //     ++current,
                //     total,
                //     productDownloads.Title);

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.FindAll(
                    d =>
                    d.Type == typeof(Type).ToString()).ToArray();

                actionLogController.StartAction($"Download {typeof(Type)} entries");

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    var sanitizedUri = entry.SourceUri;
                    if (sanitizedUri.Contains(Separators.QueryString))
                        sanitizedUri = sanitizedUri.Substring(0, sanitizedUri.IndexOf(Separators.QueryString, System.StringComparison.Ordinal));

                   actionLogController.IncrementActionProgress();

                    await downloadProductFileAsyncDelegate?.DownloadProductFileAsync(
                        productDownloads.Id,
                        productDownloads.Title,
                        sanitizedUri,
                        entry.Destination);

                    actionLogController.StartAction($"Remove scheduled {typeof(Type)} downloaded entry");

                    productDownloads.Downloads.Remove(entry);
                    await productDownloadsDataController.UpdateAsync(productDownloads);

                    actionLogController.CompleteAction();
                }

                // if there are no scheduled downloads left - mark file for removal
                if (productDownloads.Downloads.Count == 0)
                    emptyProductDownloads.Add(productDownloads);

                actionLogController.CompleteAction();
            }

            actionLogController.StartAction("Clear empty downloads");

            foreach (var productDownload in emptyProductDownloads)
                await productDownloadsDataController.DeleteAsync(productDownload);

            actionLogController.CompleteAction();

            actionLogController.CompleteAction();
        }
    }
}
