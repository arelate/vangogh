using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Status;

namespace Interfaces.Cookies
{
    public interface ISetCookiesAsyncDelegate
    {
        Task SetCookiesAsync(IEnumerable<string> cookies, IStatus status);
    }

    public interface IGetCookiesStringAsyncDelegate
    {
        Task<string> GetCookiesStringAsync(IStatus status);
    }

    public interface ICookieController:
        ISetCookiesAsyncDelegate,
        IGetCookiesStringAsyncDelegate,
        IDataAvailableDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate
    {
        // ...
    }
}
