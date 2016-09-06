using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.Network;

namespace Controllers.Network
{
    public sealed class NetworkController : INetworkController
    {
        private HttpClient client;
        private IUriController uriController;
        const string postMediaType = "application/x-www-form-urlencoded";

        public NetworkController(IUriController uriController)
        {
            client = new HttpClient();
            this.uriController = uriController;
        }

        public async Task<string> Get(
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.ConcatenateUri(baseUri, parameters);

            using (var response = await GetResponse(uri))
            {
                if (response == null) return null;

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task<HttpResponseMessage> GetResponse(string uri)
        {
            return await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        }

        public async Task<string> Post(
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.ConcatenateUri(baseUri, parameters);

            var content = new StringContent(data, Encoding.UTF8, postMediaType);

            using (var response = await client.PostAsync(uri, content))
            {
                if (response == null) return null;

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }
    }
}
