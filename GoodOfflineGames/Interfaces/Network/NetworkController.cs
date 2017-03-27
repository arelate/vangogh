using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.TaskStatus;

namespace Interfaces.Network
{
    public interface IGetDelegate
    {
        Task<string> Get(ITaskStatus taskStatus, string uri, IDictionary<string, string> parameters = null);
    }

    public interface IGetDeserializedDelegate<T>
    {
        Task<T> GetDeserialized(ITaskStatus taskStatus, string uri, IDictionary<string, string> parameters = null);
    }

    public interface IRequestResponseDelegate
    {
        Task<HttpResponseMessage> RequestResponse(ITaskStatus taskStatus, HttpMethod method, string uri, HttpContent content = null);
    }

    public interface IPostDelegate
    {
        Task<string> Post(ITaskStatus taskStatus, string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface INetworkController :
        IGetDelegate,
        IRequestResponseDelegate,
        IPostDelegate
    {
        // ... 
    }
}
