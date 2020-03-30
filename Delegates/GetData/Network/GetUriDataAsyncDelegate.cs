using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetInstance;

namespace Delegates.GetData.Network
{
    public class GetUriDataAsyncDelegate : IGetDataAsyncDelegate<string>
    {
        private readonly IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate;

        [Dependencies(
            "Delegates.GetInstance.Network.GetHttpClientInstanceDelegate,Delegates")]
        public GetUriDataAsyncDelegate(
            IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate)
        {
            this.getHttpClientInstanceDelegate = getHttpClientInstanceDelegate;
        }

        public async Task<string> GetDataAsync(string uri = null)
        {
            var httpClient = getHttpClientInstanceDelegate.GetInstance();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            using (var response = await httpClient.SendAsync(
                requestMessage, 
                HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                    return await reader.ReadToEndAsync();
            }
        }
    }
}