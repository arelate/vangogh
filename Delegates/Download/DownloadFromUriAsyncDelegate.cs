using System;
using System.Threading.Tasks;
using System.Net.Http;
using Interfaces.Delegates.Download;
using Interfaces.Delegates.Convert;
using Attributes;

namespace Delegates.Download
{
    public class DownloadFromUriAsyncDelegate : IDownloadFromUriAsyncDelegate
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;

        private readonly IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate;

        [Dependencies(
            typeof(Convert.Network.ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate),
            typeof(DownloadFromResponseAsyncDelegate))]
        public DownloadFromUriAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
                convertRequestToResponseAsyncDelegate,
            IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate)
        {
            this.convertRequestToResponseAsyncDelegate = convertRequestToResponseAsyncDelegate;
            this.downloadFromResponseAsyncDelegate = downloadFromResponseAsyncDelegate;
        }

        public async Task DownloadFromUriAsync(string sourceUri, string destination)
        {
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
            }
        }
    }
}