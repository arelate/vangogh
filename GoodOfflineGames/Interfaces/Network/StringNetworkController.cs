using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Network
{
    public interface IGetStringDelegate
    {
        Task<string> GetString(string uri, IDictionary<string, string> parameters = null);
    }

    public interface IPostStringDelegate
    {
        Task<string> PostString(string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface IStringNetworkController :
        IGetStringDelegate,
        IPostStringDelegate
    {
        // ... 
    }
}
