using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;

namespace Interfaces.Cookies
{
    public interface ISetCookiesDelegate
    {
        Task SetCookies(string headers);
    }

    public interface IGetCookiesDelegate
    {
        IEnumerable<Cookie> GetCookies();
    }

    public interface ICookieSerializationController:
        ISetCookiesDelegate,
        IGetCookiesDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate
    {
        // ...
    }
}
