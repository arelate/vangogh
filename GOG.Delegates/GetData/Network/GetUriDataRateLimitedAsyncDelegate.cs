using System.Net.Http;
using System.Threading.Tasks;

using Attributes;

using Interfaces.Delegates.Convert;

using Delegates.GetData.Network;

namespace GOG.Delegates.GetData.Network
{
    public class GetUriDataRateLimitedAsyncDelegate:
        GetUriDataAsyncDelegate
    {
        [Dependencies(
            "GOG.Delegates.Convert.Network.ConvertHttpRequestMessageToHttpResponseMethodRateLimitedAsyncDelegate,GOG.Delegates")]
        public GetUriDataRateLimitedAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>> convertRequestToResponseAsyncDelegate) : 
            base(convertRequestToResponseAsyncDelegate)
        {
            // ...
        }
    }
}