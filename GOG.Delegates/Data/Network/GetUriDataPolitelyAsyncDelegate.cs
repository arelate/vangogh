using System.Net.Http;
using System.Threading.Tasks;
using Attributes;
using Delegates.Data.Network;
using Interfaces.Delegates.Convert;
using GOG.Delegates.Convert.Network;

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