using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.Uri;
using Interfaces.Network;
using Interfaces.Cookies;

namespace Controllers.Network
{
    public sealed class NetworkController : INetworkController
    {
        private HttpClient client;
        private ICookiesController cookiesController;
        private IUriController uriController;
        const string postMediaType = "application/x-www-form-urlencoded";
        const string acceptHeaderContent = "text/html, application/xhtml+xml, image/jxr, */*";

        const string setCookieHeader = "Set-Cookie";
        const string cookieHeader = "Cookie";
        const string acceptHeader = "Accept";

        public NetworkController(
            ICookiesController cookiesController,
            IUriController uriController)
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
        }

        public async Task SetCookies(HttpResponseMessage response)
        {
            IEnumerable<string> responseCookies = new List<string>();
            response.Headers.TryGetValues(setCookieHeader, out responseCookies);

            await cookiesController.SetCookies(responseCookies);
        }

        public async Task<string> Get(
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.ConcatenateUriWithKeyValueParameters(baseUri, parameters);

            using (var response = await RequestResponse(HttpMethod.Get, uri))
            {
                response.EnsureSuccessStatusCode();

                await SetCookies(response);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task<HttpResponseMessage> RequestResponse(HttpMethod method, string uri, HttpContent content = null)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add(cookieHeader, await cookiesController.GetCookieHeader());
            requestMessage.Headers.Add(acceptHeader, acceptHeaderContent);
            if (content != null) requestMessage.Content = content;
            return await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }

        public async Task<string> Post(
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.ConcatenateUriWithKeyValueParameters(baseUri, parameters);

            if (data == null) data = string.Empty;
            var content = new StringContent(data, Encoding.UTF8, postMediaType);

            using (var response = await RequestResponse(HttpMethod.Post, uri, content))
            {
                response.EnsureSuccessStatusCode();

                await SetCookies(response);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }
    }
}
