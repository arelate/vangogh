using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;

using Interfaces.Uri;
using Interfaces.Network;
using Interfaces.Cookies;
using Interfaces.RequestRate;
using Interfaces.Status;

using Models.Network;
using System;

namespace Controllers.Network
{
    public sealed class NetworkController : INetworkController
    {
        private HttpClient client;
        private ICookieController cookieController;
        private IUriController uriController;
        private IRequestRateController requestRateController;

        public NetworkController(
            ICookieController cookieController,
            IUriController uriController,
            IRequestRateController requestRateController)
        {
            this.cookieController = cookieController;
            this.uriController = uriController;
            this.requestRateController = requestRateController;

            var httpHandler = new HttpClientHandler()
            {
                UseDefaultCredentials = false
            };
            client = new HttpClient(httpHandler);
            client.DefaultRequestHeaders.ExpectContinue = false;
            client.DefaultRequestHeaders.Add(Headers.UserAgent, HeaderDefaultValues.UserAgent);
        }

        public async Task<string> GetAsync(
            IStatus status,
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.ConcatenateUriWithKeyValueParameters(baseUri, parameters);

            using (var response = await RequestResponseAsync(status, HttpMethod.Get, uri))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task<HttpResponseMessage> RequestResponseAsync(
            IStatus status,
            HttpMethod method,
            string uri,
            HttpContent content = null)
        {
            requestRateController.EnforceRequestRate(uri, status);

            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add(Headers.Accept, HeaderDefaultValues.Accept);
            requestMessage.Headers.Add(Headers.Cookie, cookieController.GetCookiesString());

            if (content != null) requestMessage.Content = content;
            var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            if (response.Headers.Contains(Headers.SetCookie))
                await cookieController.SetCookies(response.Headers.GetValues(Headers.SetCookie));

            return response;
        }

        public async Task<string> PostAsync(
            IStatus status,
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.ConcatenateUriWithKeyValueParameters(baseUri, parameters);

            if (data == null) data = string.Empty;
            var content = new StringContent(data, Encoding.UTF8, HeaderDefaultValues.ContentType);

            using (var response = await RequestResponseAsync(status, HttpMethod.Post, uri, content))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }
    }
}
