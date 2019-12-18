using System;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Delegates.Download;

using Interfaces.Controllers.Network;
using Interfaces.Controllers.Logs;

using Attributes;

namespace Delegates.Download
{
    public class DownloadFromUriAsyncDelegate : IDownloadFromUriAsyncDelegate
    {
        readonly IRequestResponseAsyncDelegate requestResponseAsyncDelegate;
        readonly IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "Controllers.Network.NetworkController,Controllers",
            "Delegates.Download.DownloadFromResponseAsyncDelegate,Delegates",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public DownloadFromUriAsyncDelegate(
            IRequestResponseAsyncDelegate requestResponseAsyncDelegate,
            IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate,
            IActionLogController actionLogController)
        {
            this.requestResponseAsyncDelegate = requestResponseAsyncDelegate;
            this.downloadFromResponseAsyncDelegate = downloadFromResponseAsyncDelegate;

            this.actionLogController = actionLogController;
        }

        public async Task DownloadFromUriAsync(string sourceUri, string destination)
        {
            actionLogController.StartAction("Download from source");

            try
            {
                using (var response = await requestResponseAsyncDelegate.RequestResponseAsync(HttpMethod.Get, sourceUri))
                    await downloadFromResponseAsyncDelegate.DownloadFromResponseAsync(response, destination);
            }
            catch (Exception ex)
            {
                // TODO: Replace statusController warnings
                // await statusController.WarnAsync(downloadEntryTask, $"{sourceUri}: {ex.Message}");
            }
            finally
            {
                actionLogController.CompleteAction();
            }
        }
    }
}
