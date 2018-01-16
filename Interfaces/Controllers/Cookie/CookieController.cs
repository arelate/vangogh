using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Status;

namespace Interfaces.Controllers.Cookies
{
    public interface ISetCookiesAsyncDelegate
    {
        Task SetCookiesAsync(IEnumerable<string> cookies, IStatus status);
    }

    public interface IGetCookiesStringAsyncDelegate
    {
        Task<string> GetCookiesStringAsync(IStatus status);
    }

    public interface ICookiesController:
        ISetCookiesAsyncDelegate,
        IGetCookiesStringAsyncDelegate
    {
        // ...
    }
}
