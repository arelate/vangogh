using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Text;
using Attributes;
using Interfaces.Delegates.PostData;
using Interfaces.Delegates.Convert;
using Models.Network;

namespace Delegates.PostData.Network
{
    public class PostUriDataAsyncDelegate : IPostDataAsyncDelegate<string>
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;

        [Dependencies(
            "Delegates.Convert.Network.ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate,Delegates")]
        public PostUriDataAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
                convertRequestToResponseAsyncDelegate)
        {
            this.convertRequestToResponseAsyncDelegate = convertRequestToResponseAsyncDelegate;
        }

        public async Task<string> PostDataAsync(string data, string uri = null)
        {
            if (data == null) data = string.Empty;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(
                    data,
                    Encoding.UTF8,
                    HeaderDefaultValues.ContentType)
            };

            using var response = await convertRequestToResponseAsyncDelegate.ConvertAsync(
                requestMessage);

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}