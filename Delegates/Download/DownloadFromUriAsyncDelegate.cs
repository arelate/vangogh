using System;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Delegates.Download;
using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Logs;


using Attributes;

namespace Delegates.Download
{
    public class DownloadFromUriAsyncDelegate : IDownloadFromUriAsyncDelegate
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;
        readonly IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "Delegates.Convert.Network.ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate,Delegates",
            "Delegates.Download.DownloadFromResponseAsyncDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public DownloadFromUriAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
                convertRequestToResponseAsyncDelegate,
            IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate,
            IActionLogController actionLogController)
        {
            this.convertRequestToResponseAsyncDelegate = convertRequestToResponseAsyncDelegate;
            this.downloadFromResponseAsyncDelegate = downloadFromResponseAsyncDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task DownloadFromUriAsync(string sourceUri, string destination)
        {
            actionLogController.StartAction("Download from source");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, sourceUri);
                using var response = await convertRequestToResponseAsyncDelegate.ConvertAsync(request);
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
