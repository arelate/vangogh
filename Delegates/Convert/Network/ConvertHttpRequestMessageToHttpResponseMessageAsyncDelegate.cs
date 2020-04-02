using System.Net.Http;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetInstance;

namespace Delegates.Convert.Network
{
    public class ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate :
        IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
    {
        private readonly IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate;

        public ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate(
            IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate)
        {
            this.getHttpClientInstanceDelegate = getHttpClientInstanceDelegate;
        }

        public async Task<HttpResponseMessage> ConvertAsync(HttpRequestMessage requestMessage)
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