using System.Net.Http;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.GetInstance;
using Interfaces.Delegates.Constrain;
using Delegates.Convert.Network;
using Delegates.GetInstance.Network;

namespace GOG.Delegates.Convert.Network
{
    public class ConvertHttpRequestMessageToHttpResponseMethodRateLimitedAsyncDelegate :
        ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate
    {
        private readonly IConstrainAsyncDelegate<string> constrainUriAsyncDelegate;

        [Dependencies(
            typeof(GetHttpClientInstanceDelegate),
            typeof(GOG.Delegates.Constrain.Network.ConstrainGOGRequestRateAsyncDelegate))]
        public ConvertHttpRequestMessageToHttpResponseMethodRateLimitedAsyncDelegate(
            IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate,
            IConstrainAsyncDelegate<string> constrainUriAsyncDelegate) :
            base(getHttpClientInstanceDelegate)
        {
            this.constrainUriAsyncDelegate = constrainUriAsyncDelegate;
        }

        public override async Task<HttpResponseMessage> ConvertAsync(HttpRequestMessage request)
        {
            await constrainUriAsyncDelegate.ConstrainAsync(request.RequestUri.AbsoluteUri);

            return await base.ConvertAsync(request);
        }
    }
}