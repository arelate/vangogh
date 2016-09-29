using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Cookies
{
    public interface IGetNameDelegate
    {
        string GetName(string cookie);
    }

    public interface IGetValueDelegate
    {
        string GetValue(string cookie);
    }

    public interface IGetCookieHeader
    {
        Task<string> GetCookieHeader();
    }

    public interface ISetCookiesDelegate
    {
        Task SetCookies(IEnumerable<string> setCookieHeader);
    }

    public interface ICookiesController:
        IGetNameDelegate,
        IGetCookieHeader,
        ISetCookiesDelegate
    {
        // ...
    }
}
