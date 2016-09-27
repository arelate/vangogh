using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;

using Interfaces.Network;
using Interfaces.Cookies;

namespace Controllers.Network
{
    public sealed class NetworkController : INetworkController
    {
        private HttpClient client;
        private CookieContainer cookieContainer;
        private ICookiesController cookiesController;
        private IUriController uriController;
        const string postMediaType = "application/x-www-form-urlencoded";
        const string userAgentHeader = "User-Agent";
        const string userAgentString = "Mozilla/5.0 (iPad; CPU OS 9_2_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13D15 Safari/601.1";

        public NetworkController(
            ICookiesController cookiesController,
            IUriController uriController)
        {
            cookieContainer = new CookieContainer();
            var httpHandler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            client = new HttpClient(httpHandler);
            client.DefaultRequestHeaders.Add(userAgentHeader, userAgentString);

            this.cookiesController = cookiesController;
            this.uriController = uriController;
        }

        public async Task<string> Get(
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.ConcatenateUri(baseUri, parameters);

            using (var response = await GetResponse(uri))
            {
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task<HttpResponseMessage> GetResponse(string uri)
        {
            if (cookiesController != null &&
                cookieContainer.Count == 0)
            {
                foreach (var cookie in await cookiesController.GetCookies())
                    cookieContainer.Add(cookie);
            }

            return await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        }

        // TODO: FormUrlEncodedContent
        public async Task<string> Post(
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.ConcatenateUri(baseUri, parameters);

            var content = new StringContent(data, Encoding.UTF8, postMediaType);

            using (var response = await client.PostAsync(uri, content))
            {
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }
    }
}
