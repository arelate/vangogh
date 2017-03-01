using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //resolvedUri = response.RequestMessage.RequestUri.ToString();
        //if (updateRouteEligibilityDelegate.IsEligible(entry))
        //    await routingController.UpdateRouteAsync(
        //        productDownloads.Id,
        //        productDownloads.Title,
        //        entry.SourceUri,
        //        resolvedUri,
        //        downloadEntryTask);

        public Task DownloadFileFromSourceAsync(string sourceUri, string destination, ITaskStatus taskStatus)
        {
            throw new NotImplementedException();
        }
    }
}
