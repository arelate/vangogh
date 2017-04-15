using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Status;

namespace Interfaces.Network
{
    public interface IGetDelegate
    {
        Task<string> Get(IStatus status, string uri, IDictionary<string, string> parameters = null);
    }

    public interface IGetDeserializedDelegate<T>
    {
        Task<T> GetDeserialized(IStatus status, string uri, IDictionary<string, string> parameters = null);
    }

    public interface IRequestResponseDelegate
    {
        Task<HttpResponseMessage> RequestResponse(IStatus status, HttpMethod method, string uri, HttpContent content = null);
    }

    public interface IPostDelegate
    {
        Task<string> Post(IStatus status, string uri, IDictionary<string, string> parameters = null, string data = null, string referer = null);
    }

    public interface INetworkController :
        IGetDelegate,
        IRequestResponseDelegate,
        IPostDelegate
    {
        // ... 
    }
}
