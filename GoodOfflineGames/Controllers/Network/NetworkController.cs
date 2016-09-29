using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;

using Interfaces.Network;
using Interfaces.Cookies;
using Interfaces.Settings;

namespace Controllers.Network
{
    public sealed class NetworkController : INetworkController, IUserAgentProperty
    {
        private HttpClient client;
        private ICookiesController cookiesController;
        private IUriController uriController;
        const string postMediaType = "application/x-www-form-urlencoded";

        const string userAgentHeader = "User-Agent";
        const string setCookieHeader = "Set-Cookie";
        const string cookieHeader = "Cookie";

        const string defaultUserAgentString = "Mozilla/5.0 (iPad; CPU OS 9_2_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13D15 Safari/601.1";
        private string userAgent;

        public string UserAgent
        {
            get { return userAgent; }
            set
            {
                userAgent = value;

                if (string.IsNullOrEmpty(userAgent))
                    userAgent = defaultUserAgentString;

                if (client.DefaultRequestHeaders.Contains(userAgentHeader))
                    client.DefaultRequestHeaders.Remove(userAgentHeader);
                client.DefaultRequestHeaders.Add(userAgentHeader, userAgent);
            }
        }

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

                IEnumerable<string> responseCookies = new List<string>();
                response.Headers.TryGetValues(setCookieHeader, out responseCookies);

                await cookiesController.SetCookies(responseCookies);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task<HttpResponseMessage> GetResponse(string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            requestMessage.Headers.Add(cookieHeader, await cookiesController.GetCookieHeader());
            return await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }

        // TODO: FormUrlEncodedContent
        public async Task<string> Post(
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.ConcatenateUri(baseUri, parameters);

            if (data == null) data = string.Empty;

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
