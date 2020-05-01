using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.Network;

namespace Delegates.Data.Network
{
    public class PostUriDataAsyncDelegate : IPostDataAsyncDelegate<string>
    {
        private readonly IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>>
            convertRequestToResponseAsyncDelegate;

        [Dependencies(
            typeof(Delegates.Convert.Network.ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate))]
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