using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using Interfaces.FileDownload;
using Interfaces.Session;
using Interfaces.Routing;
using Interfaces.TaskStatus;
using Interfaces.Network;

namespace GOG.Controllers.FileDownload
{
    public class ManualUrlDownloadFromSourceDelegate : IDownloadFileFromSourceDelegate
    {
        private INetworkController networkController;
        private ISessionController sessionController;
        private IRoutingController routingController;
        private IFileDownloadController fileDownloadController;
        private ITaskStatusController taskStatusController;

        public ManualUrlDownloadFromSourceDelegate(
            INetworkController networkController,
            ISessionController sessionController,
            IRoutingController routingController,
            IFileDownloadController fileDownloadController,
            ITaskStatusController taskStatusController)
        {
            this.networkController = networkController;
            this.sessionController = sessionController;
            this.routingController = routingController;
            this.fileDownloadController = fileDownloadController;
            this.taskStatusController = taskStatusController;
        }

        public async Task DownloadFileFromSourceAsync(long id, string title, string sourceUri, string destination, ITaskStatus taskStatus)
        {
            var downloadTask = taskStatusController.Create(taskStatus, "Download game details manual url");

            using (var response = await networkController.RequestResponse(HttpMethod.Get, sourceUri))
            {
                var resolvedUri = response.RequestMessage.RequestUri.ToString();

                // GOG.com quirk
                // When resolving ManualUrl from GameDetails we get CDN Uri with the session key.
                // Storing this key is pointless - it expries after some time and needs to be updated.
                // So here we filter our this session key and store direct file Uri

                var uriSansSession = sessionController.GetUriSansSession(resolvedUri);

                await routingController.UpdateRouteAsync(
                    id,
                    title,
                    sourceUri,
                    uriSansSession,
                    downloadTask);

                try
                {
                    await fileDownloadController.DownloadFileFromResponseAsync(
                        response, 
                        destination, 
                        downloadTask);
                }
                catch (Exception ex)
                {
                    taskStatusController.Fail(
                        downloadTask, 
                        $"Couldn't download {sourceUri}, resolved as {resolvedUri} to {destination} " +
                        $"for product {id}: {title}, error message: {ex.Message}");
                }
            }

            taskStatusController.Complete(downloadTask);
        }
    }
}
