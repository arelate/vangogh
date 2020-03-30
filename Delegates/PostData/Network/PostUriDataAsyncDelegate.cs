using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Text;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetInstance;

using Models.Network;

namespace Delegates.PostData.Network
{
    public class PostUriDataAsyncDelegate : IPostDataAsyncDelegate<string>
    {
        private readonly IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate;

        [Dependencies(
            "Delegates.GetInstance.Network.GetHttpClientInstanceDelegate,Delegates")]
        public PostUriDataAsyncDelegate(
            IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate)
        {
            this.getHttpClientInstanceDelegate = getHttpClientInstanceDelegate;
        }

        public async Task<string> PostDataAsync(string data, string uri = null)
        {
            if (data == null) data = string.Empty;
            var content = new StringContent(data, Encoding.UTF8, HeaderDefaultValues.ContentType);
            var httpClient = getHttpClientInstanceDelegate.GetInstance();

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            if (content != null) requestMessage.Content = content;

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