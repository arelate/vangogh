using System.Linq;
using System.Threading.Tasks;

using Interfaces.FileDownload;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductDownloads;
using Models.Separators;

namespace GOG.TaskActivities.ProcessDownloads
{
    public class ProcessDownloadsController : TaskActivityController
    {
        private string downloadParameter;
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDownloadFileFromSourceDelegate downloadFileFromSourceDelegate;

        public ProcessDownloadsController(
            string downloadParameter,
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadFileFromSourceDelegate downloadFileFromSourceDelegate,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.downloadParameter = downloadParameter;
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.downloadFileFromSourceDelegate = downloadFileFromSourceDelegate;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var counter = 0;
            var updated = updatedDataController.EnumerateIds().ToArray();
            var total = updated.Length;

            var processDownloadsTask = taskStatusController.Create(taskStatus, "Process updated downloads");

            foreach (var id in updated)
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                taskStatusController.UpdateProgress(
                    processDownloadsTask,
                    ++counter,
                    total,
                    productDownloads.Title);

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.FindAll(d => d.DownloadParameter == downloadParameter).ToArray();

                var processDownloadEntriesTask = taskStatusController.Create(processDownloadsTask, "Download product entries");

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
                        "Remove successfully downloaded scheduled entry");

                    productDownloads.Downloads.Remove(entry);
                    await productDownloadsDataController.UpdateAsync(removeEntryTask, productDownloads);

                    taskStatusController.Complete(removeEntryTask);
                }

                taskStatusController.Complete(processDownloadEntriesTask);
            }

            taskStatusController.Complete(processDownloadsTask);
        }
    }
}
