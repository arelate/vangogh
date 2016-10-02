using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace Interfaces.Network
{
    public interface IGetDelegate
    {
        Task<string> Get(string uri, IDictionary<string, string> parameters = null);
    }

    public interface IGetResponseDelegate
    {
        Task<HttpResponseMessage> GetResponse(HttpMethod method, string uri, HttpContent content = null);
    }

    public interface IPostDelegate
    {
        Task<string> Post(string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface INetworkController :
        IGetDelegate,
        IGetResponseDelegate,
        IPostDelegate
    {
        // ... 
    }
}
