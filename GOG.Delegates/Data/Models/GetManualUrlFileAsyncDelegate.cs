using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Download;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;
using Models.ProductTypes;
using Delegates.Convert.Network;
using Delegates.Format.Uri;
using Delegates.Data.Routes;
using Delegates.Download;
using Delegates.Activities;
using GOG.Models;

namespace GOG.Delegates.Data.Models
{
    public class GetManualUrlFileAsyncDelegate : IGetDataAsyncDelegate<string, ProductFileDownloadManifest>
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;

        private readonly IFormatDelegate<string, string> formatUriRemoveSessionDelegate;
        private readonly IUpdateAsyncDelegate<ProductRoutes> updateRouteDataAsyncDelegate;
        private readonly IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate;
        private readonly IGetDataAsyncDelegate<string, ProductFileDownloadManifest> getValidationFileAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate),
            typeof(FormatUriRemoveSessionDelegate),
            typeof(UpdateRouteDataAsyncDelegate),
            typeof(DownloadFromResponseAsyncDelegate),
            typeof(GetValidationFileAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public GetManualUrlFileAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
                convertRequestToResponseAsyncDelegate,
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate,
            IUpdateAsyncDelegate<ProductRoutes> updateRouteDataAsyncDelegate,
            IDownloadFromResponseAsyncDelegate downloadFromResponseAsyncDelegate,
            IGetDataAsyncDelegate<string, ProductFileDownloadManifest> getValidationFileAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.convertRequestToResponseAsyncDelegate = convertRequestToResponseAsyncDelegate;
            this.formatUriRemoveSessionDelegate = formatUriRemoveSessionDelegate;
            this.updateRouteDataAsyncDelegate = updateRouteDataAsyncDelegate;
            this.downloadFromResponseAsyncDelegate = downloadFromResponseAsyncDelegate;
            this.getValidationFileAsyncDelegate = getValidationFileAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<string> GetDataAsync(ProductFileDownloadManifest productFileDownloadManifest)
        {
            startDelegate.Start("Download game details manual url");

            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, productFileDownloadManifest.Source);
                response = await convertRequestToResponseAsyncDelegate.ConvertAsync(request);
            }
            catch (HttpRequestException ex)
            {
                // await statusController.FailAsync(
                //     downloadTask,
                //     $"Failed to get successful response for {sourceUri} for " +
                //     $"product {id}: {title}, message: {ex.Message}");
                completeDelegate.Complete();
                return string.Empty;
            }

            using (response)
            {
                var resolvedUri = response.RequestMessage.RequestUri.ToString();

                // GOG.com quirk
                // When resolving ManualUrl from GameDetails we get CDN Uri with the session key.
                // Storing this key is pointless - it expires after some time and needs to be updated.
                // So here we filter our this session key and store direct file Uri

                var uriSansSession = formatUriRemoveSessionDelegate.Format(resolvedUri);

                await updateRouteDataAsyncDelegate.UpdateAsync(
                    new ProductRoutes()
                    {
                        Id = productFileDownloadManifest.Id,
                        Title = productFileDownloadManifest.Title,
                        Routes = new List<ProductRoutesEntry>()
                        {
                            new ProductRoutesEntry()
                            {
                                Source = productFileDownloadManifest.Source,
                                Destination = uriSansSession
                            }
                        }
                    });

                try
                {
                    await downloadFromResponseAsyncDelegate.DownloadFromResponseAsync(
                        response,
                        productFileDownloadManifest.Destination);
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

                if (getValidationFileAsyncDelegate != null)
                {
                    await getValidationFileAsyncDelegate.GetDataAsync(
                        new ProductFileDownloadManifest(
                            productFileDownloadManifest.Id,
                            productFileDownloadManifest.Title,
                            resolvedUri,
                            productFileDownloadManifest.Destination));
                }
            }

            completeDelegate.Complete();

            return productFileDownloadManifest.Destination;
        }
    }
}