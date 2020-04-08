using System.Net.Http;
using System.Threading.Tasks;
using Attributes;
using Delegates.Data.Network;
using Interfaces.Delegates.Convert;

namespace GOG.Delegates.Data.Network
{
    public class GetUriDataRateLimitedAsyncDelegate :
        GetUriDataAsyncDelegate
    {
        [Dependencies(
            "GOG.Delegates.Convert.Network.ConvertHttpRequestMessageToHttpResponseMethodRateLimitedAsyncDelegate,GOG.Delegates")]
        public GetUriDataRateLimitedAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>> convertRequestToResponseAsyncDelegate)
            :
            base(convertRequestToResponseAsyncDelegate)
        {
            // ...
        }
    }
}