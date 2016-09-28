using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Cookies
{
    public interface IGetNameDelegate
    {
        string GetName(string cookie);
    }

    public interface IGetCookieHeaderDelegate
    {
        Task<string> GetCookieHeader();
    }

    public interface IUpdateCookiesDelegate
    {
        Task UpdateCookies(IEnumerable<string> cookies);
    }

    public interface ICookiesController:
        IGetNameDelegate,
        IGetCookieHeaderDelegate,
        IUpdateCookiesDelegate
    {
        // ...
    }
}
