using System;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Delegates.Download;

using Interfaces.Controllers.Network;

using Interfaces.Status;

namespace Delegates.Download
{
    public class DownloadFromUriAsyncDelegate : IDownloadFromUriAsyncDelegate
    {
        readonly IRequestResponseAsyncDelegate requestResponseAsyncDelegate;
        readonly IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate;
        readonly IStatusController statusController;

        public DownloadFromUriAsyncDelegate(
            IRequestResponseAsyncDelegate requestResponseAsyncDelegate,
            IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate,
            IStatusController statusController)
        {
            this.requestResponseAsyncDelegate = requestResponseAsyncDelegate;
            this.downloadFromResponseAsyncDelegate = downloadFromResponseAsyncDelegate;

            this.statusController = statusController;
        }

        public async Task DownloadFromUriAsync(string sourceUri, string destination, IStatus status)
        {
            var downloadEntryTask = await statusController.CreateAsync(status, "Download from source");

            try
            {
                using (var response = await requestResponseAsyncDelegate.RequestResponseAsync(downloadEntryTask, HttpMethod.Get, sourceUri))
                    await downloadFromResponseAsyncDelegate.DownloadFromResponseAsync(response, destination, downloadEntryTask);
            }
            catch (Exception ex)
            {
                await statusController.WarnAsync(downloadEntryTask, $"{sourceUri}: {ex.Message}");
            }
            finally
            {
                await statusController.CompleteAsync(downloadEntryTask);
            }
        }
    }
}
