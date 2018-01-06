using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;

namespace Interfaces.Cookies
{
    public interface ISetCookiesDelegate
    {
        Task SetCookies(IEnumerable<string> cookies);
    }

    public interface IGetCookiesDelegate
    {
        IEnumerable<Cookie> GetCookies();
    }

    public interface ICookieController:
        ISetCookiesDelegate,
        IGetCookiesDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate
    {
        // ...
    }
}
