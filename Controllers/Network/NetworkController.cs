using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.Delegates.Constrain;

using Interfaces.Controllers.Network;

using Attributes;

namespace Controllers.Network
{
    public sealed class NetworkController : INetworkController
    {
        HttpClient client;
        IConstrainAsyncDelegate<string> constrainRequestRateAsyncDelegate;

        [Dependencies(
            "Delegates.Constrain.ConstrainRequestRateAsyncDelegate,Delegates")]
        public NetworkController(
            IConstrainAsyncDelegate<string> constrainRequestRateAsyncDelegate)
        {
            this.constrainRequestRateAsyncDelegate = constrainRequestRateAsyncDelegate;
        }

        public async Task<HttpResponseMessage> RequestResponseAsync(
            HttpMethod method,
            string uri,
            HttpContent content = null)
        {
            await constrainRequestRateAsyncDelegate.ConstrainAsync(uri);

            var requestMessage = new HttpRequestMessage(method, uri);

            if (content != null) requestMessage.Content = content;
            var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
