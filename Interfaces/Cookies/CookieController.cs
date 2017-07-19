using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;

namespace Interfaces.Cookies
{
    public interface ISetCookiesDelegate
    {
        Task SetCookies(IEnumerable<string> cookies);
    }

    public interface IGetCookiesStringDelegate
    {
        string GetCookiesString();
    }

    public interface ICookieController:
        ISetCookiesDelegate,
        IGetCookiesStringDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate
    {
        // ...
    }
}
