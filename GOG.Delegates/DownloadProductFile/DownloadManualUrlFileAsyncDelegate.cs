using System;
using System.Net.Http;
using System.Threading.Tasks;

using Interfaces.Delegates.Format;
using Interfaces.Delegates.Download;
using Interfaces.Delegates.Itemize;
using Interfaces.Models.Dependencies;

using Interfaces.Controllers.Network;
using Interfaces.Controllers.Logs;

using Interfaces.Routing;


using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

namespace GOG.Delegates.DownloadProductFile
{
    public class DownloadManualUrlFileAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        readonly INetworkController networkController;
        readonly IFormatDelegate<string, string> formatUriRemoveSessionDelegate;
        readonly IRoutingController routingController;
        readonly IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate;
        readonly IActionLogController actionLogController;
        readonly IDownloadProductFileAsyncDelegate downloadValidationFileAsyncDelegate;

		[Dependencies(
            DependencyContext.Default,
			"Controllers.Network.NetworkController,Controllers",
			"Delegates.Format.Uri.FormatUriRemoveSessionDelegate,Delegates",
			"Controllers.Routing.RoutingController,Controllers",
			"Delegates.Download.DownloadFromResponseAsyncDelegate,Delegates",
			"GOG.Delegates.DownloadProductFile.DownloadValidationFileAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]
        public DownloadManualUrlFileAsyncDelegate(
            INetworkController networkController,
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate,
            IRoutingController routingController,
            IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate,
            IDownloadProductFileAsyncDelegate downloadValidationFileAsyncDelegate,
            IActionLogController actionLogController)
        {
            this.networkController = networkController;
            this.formatUriRemoveSessionDelegate = formatUriRemoveSessionDelegate;
            this.routingController = routingController;
            this.downloadFromResponseAsyncDelegate = downloadFromResponseAsyncDelegate;
            this.downloadValidationFileAsyncDelegate = downloadValidationFileAsyncDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task DownloadProductFileAsync(long id, string title, string sourceUri, string destination)
        {
           actionLogController.StartAction("Download game details manual url");

            HttpResponseMessage response;
            try
            {
                response = await networkController.RequestResponseAsync(HttpMethod.Get, sourceUri);
            }
            catch (HttpRequestException ex)
            {
                // await statusController.FailAsync(
                //     downloadTask,
                //     $"Failed to get successful response for {sourceUri} for " +
                //     $"product {id}: {title}, message: {ex.Message}");
                actionLogController.CompleteAction();
                return;
            }

            using (response)
            {

                var resolvedUri = response.RequestMessage.RequestUri.ToString();

                // GOG.com quirk
                // When resolving ManualUrl from GameDetails we get CDN Uri with the session key.
                // Storing this key is pointless - it expries after some time and needs to be updated.
                // So here we filter our this session key and store direct file Uri

                var uriSansSession = formatUriRemoveSessionDelegate.Format(resolvedUri);

                await routingController.UpdateRouteAsync(
                    id,
                    title,
                    sourceUri,
                    uriSansSession);

                try
                {
                    await downloadFromResponseAsyncDelegate.DownloadFromResponseAsync(
                        response, 
                        destination);
                }
                catch (Exception ex)
                {
                    // await statusController.FailAsync(
                    //     downloadTask, 
                    //     $"Couldn't download {sourceUri}, resolved as {resolvedUri} to {destination} " +
                    //     $"for product {id}: {title}, error message: {ex.Message}");
                }

                // GOG.com quirk
                // Supplementary download is a secondary download to a primary driven by download scheduling
                // The example is validation file - while we can use the same pipeline, we would be
                // largerly duplicating all the work to establish the session, compute the name etc.
                // While the only difference validation files have - is additional extension.
                // So instead we'll do a supplementary download using primary download information

                await downloadValidationFileAsyncDelegate?.DownloadProductFileAsync(
                    id, 
                    title, 
                    resolvedUri,
                    destination);
            }

            actionLogController.CompleteAction();
        }
    }
}
