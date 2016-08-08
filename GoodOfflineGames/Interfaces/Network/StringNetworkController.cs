using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Network
{
    public interface IGetDelegate
    {
        Task<string> Get(string uri, IDictionary<string, string> parameters = null);
    }

    public interface IPostDelegate
    {
        Task<string> Post(string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface INetworkController :
        IGetDelegate,
        IPostDelegate
    {
        // ... 
    }
}
