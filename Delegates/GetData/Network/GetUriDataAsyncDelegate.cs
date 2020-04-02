using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Attributes;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.Convert;

namespace Delegates.GetData.Network
{
    public class GetUriDataAsyncDelegate : IGetDataAsyncDelegate<string>
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;

        [Dependencies(
            "Delegates.GetInstance.Network.GetHttpClientInstanceDelegate,Delegates")]
        public GetUriDataAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
                convertRequestToResponseAsyncDelegate)
        {
            this.convertRequestToResponseAsyncDelegate = convertRequestToResponseAsyncDelegate;
        }

        public async Task<string> GetDataAsync(string uri = null)
        {
            // var httpClient = getHttpClientInstanceDelegate.GetInstance();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            using var response = await convertRequestToResponseAsyncDelegate.ConvertAsync(requestMessage);

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}