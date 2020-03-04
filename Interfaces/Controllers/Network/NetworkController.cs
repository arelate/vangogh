using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace Interfaces.Controllers.Network
{
    public interface IGetResourceAsyncDelegate
    {
        Task<string> GetResourceAsync(string uri, IDictionary<string, string> parameters = null);
    }

    public interface IRequestResponseAsyncDelegate
    {
        Task<HttpResponseMessage> RequestResponseAsync(HttpMethod method, string uri, HttpContent content = null);
    }

    public interface IPostDataToResourceAsyncDelegate
    {
        Task<string> PostDataToResourceAsync(string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface INetworkController :
        IGetResourceAsyncDelegate,
        IRequestResponseAsyncDelegate,
        IPostDataToResourceAsyncDelegate
    {
        // ... 
    }
}
