using System.Net.Http;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.GetInstance;
using Interfaces.Delegates.Throttling;
using Delegates.Convert.Network;
using Delegates.GetInstance.Network;
using GOG.Delegates.Throttling.Network;

namespace GOG.Delegates.Convert.Network
{
    public class ConvertHttpRequestMessageToHttpResponseMethodPolitelyAsyncDelegate :
        ConvertHttpRequestMessageToHttpResponseMessageAsyncDelegate
    {
        private readonly IThrottleAsyncDelegate<string> throttleUriAsyncDelegate;

        [Dependencies(
            typeof(GetHttpClientInstanceDelegate),
            typeof(ThrottleGOGRequestRateAsyncDelegate))]
        public ConvertHttpRequestMessageToHttpResponseMethodPolitelyAsyncDelegate(
            IGetInstanceDelegate<HttpClient> getHttpClientInstanceDelegate,
            IThrottleAsyncDelegate<string> throttleUriAsyncDelegate) :
            base(getHttpClientInstanceDelegate)
        {
            this.throttleUriAsyncDelegate = throttleUriAsyncDelegate;
        }

        public override async Task<HttpResponseMessage> ConvertAsync(HttpRequestMessage request)
        {
            await throttleUriAsyncDelegate.ThrottleAsync(request.RequestUri.AbsoluteUri);

            return await base.ConvertAsync(request);
        }
    }
}