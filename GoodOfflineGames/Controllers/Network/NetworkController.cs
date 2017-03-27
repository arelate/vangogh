using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.Uri;
using Interfaces.Network;
using Interfaces.Cookies;
using Interfaces.RequestRate;
using Interfaces.TaskStatus;

using Models.Network;

namespace Controllers.Network
{
    public sealed class NetworkController : INetworkController
    {
        private HttpClient client;
        private ICookiesController cookiesController;
        private IUriController uriController;
        private IRequestRateController requestRateController;

        public NetworkController(
            ICookiesController cookiesController,
            IUriController uriController,
            IRequestRateController requestRateController)
        {
            var httpHandler = new HttpClientHandler()
            {
                UseCookies = false,
                UseDefaultCredentials = false
            };
            client = new HttpClient(httpHandler);
            client.DefaultRequestHeaders.ExpectContinue = false;

            this.cookiesController = cookiesController;
            this.uriController = uriController;
            this.requestRateController = requestRateController;
        }

        public async Task<string> Get(
            ITaskStatus taskStatus,
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.ConcatenateUriWithKeyValueParameters(baseUri, parameters);

            using (var response = await RequestResponse(taskStatus, HttpMethod.Get, uri))
            {
                response.EnsureSuccessStatusCode();

                await cookiesController.SetCookies(response);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task<HttpResponseMessage> RequestResponse(
            ITaskStatus taskStatus,
            HttpMethod method, 
            string uri, 
            HttpContent content = null)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add(Headers.Cookie, await cookiesController.GetCookieHeader());
            requestMessage.Headers.Add(Headers.Accept, HeaderDefaultValues.Accept);
            if (content != null) requestMessage.Content = content;
            return await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }

        public async Task<string> Post(
            ITaskStatus taskStatus,
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.ConcatenateUriWithKeyValueParameters(baseUri, parameters);

            if (data == null) data = string.Empty;
            var content = new StringContent(data, Encoding.UTF8, HeaderDefaultValues.ContentType);

            using (var response = await RequestResponse(taskStatus, HttpMethod.Post, uri, content))
            {
                response.EnsureSuccessStatusCode();

                await cookiesController.SetCookies(response);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }
    }
}
