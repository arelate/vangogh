using System.Net.Http;
using System.Threading.Tasks;
using Attributes;
using Delegates.Data.Network;
using GOG.Delegates.Conversions.Network;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Network
{
    public class GetUriDataPolitelyAsyncDelegate :
        GetUriDataAsyncDelegate
    {
        [Dependencies(
            typeof(ConvertHttpRequestMessageToHttpResponseMethodPolitelyAsyncDelegate))]
        public GetUriDataPolitelyAsyncDelegate(
            IConvertAsyncDelegate<HttpRequestMessage, Task<HttpResponseMessage>> convertRequestToResponseAsyncDelegate)
            :
            base(convertRequestToResponseAsyncDelegate)
        {
            // ...
        }
    }
}