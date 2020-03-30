using System.Threading.Tasks;
using System.Net.Http;

namespace Interfaces.Controllers.Network
{
    public interface IRequestResponseAsyncDelegate
    {
        Task<HttpResponseMessage> RequestResponseAsync(HttpMethod method, string uri, HttpContent content = null);
    }

    public interface INetworkController :
        IRequestResponseAsyncDelegate
    {
        // ... 
    }
}
