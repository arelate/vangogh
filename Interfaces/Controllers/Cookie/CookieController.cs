using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;

namespace Interfaces.Controllers.Cookies
{
    public interface ISetCookiesAsyncDelegate
    {
        Task SetCookiesAsync(IEnumerable<string> cookies);
    }

    public interface IGetCookiesStringAsyncDelegate
    {
        Task<string> GetCookiesStringAsync();
    }

    public interface ICookiesController:
        ISetCookiesAsyncDelegate,
        IGetCookiesStringAsyncDelegate
    {
        // ...
    }
}
