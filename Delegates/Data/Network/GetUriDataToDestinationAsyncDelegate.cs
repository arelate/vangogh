using System;
using System.Threading.Tasks;
using System.Net.Http;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Conversions;
using Attributes;
using Delegates.Conversions.Network;

namespace Delegates.Data.Network
{
    public class GetUriDataToDestinationAsyncDelegate : IGetDataToDestinationAsyncDelegate<string, string>
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;

        private readonly IGetDataToDestinationAsyncDelegate<HttpResponseMessage, string> 
            getHttpResponseMessageToDestinationAsyncDelegate;

        [Dependencies(
            typeof(ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate),
            typeof(GetHttpResponseMessageToDestinationAsyncDelegate))]
        public GetUriDataToDestinationAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
                convertRequestToResponseAsyncDelegate,
            IGetDataToDestinationAsyncDelegate<HttpResponseMessage, string> 
                getHttpResponseMessageToDestinationAsyncDelegate)
        {
            this.convertRequestToResponseAsyncDelegate = convertRequestToResponseAsyncDelegate;
            this.getHttpResponseMessageToDestinationAsyncDelegate = getHttpResponseMessageToDestinationAsyncDelegate;
        }

        public async Task GetDataToDestinationAsyncDelegate(string sourceUri, string destination)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, sourceUri);
                using var response = await convertRequestToResponseAsyncDelegate.ConvertAsync(request);
                await getHttpResponseMessageToDestinationAsyncDelegate.GetDataToDestinationAsyncDelegate(
                    response, 
                    destination);
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