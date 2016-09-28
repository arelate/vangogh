using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Cookies
{
    public interface IGetNameDelegate
    {
        string GetName(string cookie);
    }

    public interface IGetCookiesDelegate
    {
        Task<IEnumerable<string>> GetCookies();
    }

    public interface IUpdateCookiesDelegate
    {
        Task UpdateCookies(IEnumerable<string> cookies);
    }

    public interface ICookiesController:
        IGetNameDelegate,
        IGetCookiesDelegate,
        IUpdateCookiesDelegate
    {
        // ...
    }
}
