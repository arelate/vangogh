using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Controllers.Data;

using Interfaces.Status;

namespace Interfaces.Controllers.Network
{
    public interface IGetAsyncDelegate
    {
        Task<string> GetAsync(IStatus status, string uri, IDictionary<string, string> parameters = null);
    }

    public interface IRequestResponseAsyncDelegate
    {
        Task<HttpResponseMessage> RequestResponseAsync(IStatus status, HttpMethod method, string uri, HttpContent content = null);
    }

    public interface IPostAsyncDelegate
    {
        Task<string> PostAsync(IStatus status, string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface INetworkController :
        IGetAsyncDelegate,
        IRequestResponseAsyncDelegate,
        IPostAsyncDelegate
    {
        // ... 
    }
}
