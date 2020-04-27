using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Network
{
    public class GetUriDataAsyncDelegate : IGetDataAsyncDelegate<string, string>
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;

        [Dependencies(
            "Delegates.Convert.Network.ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate,Delegates")]
        public GetUriDataAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
                convertRequestToResponseAsyncDelegate)
        {
            this.convertRequestToResponseAsyncDelegate = convertRequestToResponseAsyncDelegate;
        }

        public async Task<string> GetDataAsync(string uri = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            using var response = await convertRequestToResponseAsyncDelegate.ConvertAsync(requestMessage);

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}