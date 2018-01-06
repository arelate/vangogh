using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Status;

namespace Interfaces.Controllers.Network
{
    public interface IGetResourceAsyncDelegate
    {
        Task<string> GetResourceAsync(IStatus status, string uri, IDictionary<string, string> parameters = null);
    }

    public interface IRequestResponseAsyncDelegate
    {
        Task<HttpResponseMessage> RequestResponseAsync(IStatus status, HttpMethod method, string uri, HttpContent content = null);
    }

    public interface IPostDataToResourceAsyncDelegate
    {
        Task<string> PostDataToResourceAsync(IStatus status, string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface INetworkController :
        IGetResourceAsyncDelegate,
        IRequestResponseAsyncDelegate,
        IPostDataToResourceAsyncDelegate
    {
        // ... 
    }
}
