using System.Net.Http;
using System.Threading.Tasks;
using Attributes;
using Delegates.Values.Network;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Values;

namespace Delegates.Conversions.Network
{
    public class ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate :
        IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
    {
        private readonly IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate;

        [Dependencies(
            typeof(GetHttpClientInstanceDelegate))]
        public ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate(
            IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate)
        {
            this.getHttpClientInstanceDelegate = getHttpClientInstanceDelegate;
        }

        public virtual async Task<HttpResponseMessage> ConvertAsync(HttpRequestMessage requestMessage)
        {
            var httpClient = getHttpClientInstanceDelegate.GetInstance();

            var response = await httpClient.SendAsync(
                requestMessage,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}