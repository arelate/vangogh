using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using Interfaces.FileDownload;
using Interfaces.Routing;
using Interfaces.TaskStatus;
using Interfaces.Network;

namespace GOG.Controllers.FileDownload
{
    public class ManualUrlDownloadFromSourceDelegate : IDownloadFileFromSourceDelegate
    {
        private INetworkController networkController;
        private IRoutingController routingController;
        private IFileDownloadController fileDownloadController;
        private ITaskStatusController taskStatusController;

        public ManualUrlDownloadFromSourceDelegate(
            INetworkController networkController,
            IRoutingController routingController,
            IFileDownloadController fileDownloadController,
            ITaskStatusController taskStatusController)
        {
            this.networkController = networkController;
            this.routingController = routingController;
            this.fileDownloadController = fileDownloadController;
            this.taskStatusController = taskStatusController;
        }

        public async Task DownloadFileFromSourceAsync(long id, string title, string sourceUri, string destination, ITaskStatus taskStatus)
        {
            var downloadTask = taskStatusController.Create(taskStatus, "Download manual url");

            using (var response = await networkController.RequestResponse(HttpMethod.Head, sourceUri))
            {
                var resolvedUri = response.RequestMessage.RequestUri.ToString();

                await routingController.UpdateRouteAsync(
                    id,
                    title,
                    sourceUri,
                    resolvedUri,
                    downloadTask);

                await fileDownloadController.DownloadFileFromResponseAsync(response, destination, downloadTask);
            }

            taskStatusController.Complete(downloadTask);
        }
    }
}
