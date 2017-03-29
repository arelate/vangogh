using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.FileDownload;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductDownloads;
using Models.Separators;

namespace GOG.TaskActivities.Download
{
    public class DownloadActivity : TaskActivityController
    {
        private string downloadParameter;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDownloadFileFromSourceDelegate downloadFileFromSourceDelegate;

        public DownloadActivity(
            string downloadParameter,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadFileFromSourceDelegate downloadFileFromSourceDelegate,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.downloadParameter = downloadParameter;
            this.productDownloadsDataController = productDownloadsDataController;
            this.downloadFileFromSourceDelegate = downloadFileFromSourceDelegate;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var current = 0;
            var productDownloadsData = productDownloadsDataController.EnumerateIds();
            var total = productDownloadsDataController.Count();

            var processDownloadsTask = taskStatusController.Create(taskStatus, 
                $"Process updated {downloadParameter} downloads");

            var emptyProductDownloads = new List<ProductDownloads>();

            foreach (var id in productDownloadsData)
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                taskStatusController.UpdateProgress(
                    processDownloadsTask,
                    ++current,
                    total,
                    productDownloads.Title);

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.FindAll(
                    d => 
                    d.DownloadParameter == downloadParameter).ToArray();

                var processDownloadEntriesTask = taskStatusController.Create(processDownloadsTask, 
                    $"Download {downloadParameter} entries");

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    var sanitizedUri = entry.SourceUri;
                    if (sanitizedUri.Contains(Separators.QueryString))
                        sanitizedUri = sanitizedUri.Substring(0, sanitizedUri.IndexOf(Separators.QueryString));

                    taskStatusController.UpdateProgress(
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

                    var removeEntryTask = taskStatusController.Create(
                        processDownloadEntriesTask,
                        $"Remove scheduled {downloadParameter} downloaded entry");

                    productDownloads.Downloads.Remove(entry);
                    await productDownloadsDataController.UpdateAsync(removeEntryTask, productDownloads);

                    taskStatusController.Complete(removeEntryTask);
                }

                // if there are no scheduled downloads left - mark file for removal
                if (productDownloads.Downloads.Count == 0)
                    emptyProductDownloads.Add(productDownloads);

                taskStatusController.Complete(processDownloadEntriesTask);
            }

            var clearEmptyDownloadsTask = taskStatusController.Create(processDownloadsTask, "Clear empty downloads");
            await productDownloadsDataController.RemoveAsync(clearEmptyDownloadsTask, emptyProductDownloads.ToArray());
            taskStatusController.Complete(clearEmptyDownloadsTask);

            taskStatusController.Complete(processDownloadsTask);
        }
    }
}
