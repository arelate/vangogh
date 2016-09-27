using System.Net;
using System.Threading.Tasks;

namespace Interfaces.Cookies
{
    public interface IGetCookiesDelegate
    {
        Task<Cookie[]> GetCookies();
    }

    public interface IUpdateCookiesDelegate
    {
        Task UpdateCookies(Cookie[] cookies);
    }

    public interface ICookiesController:
        IGetCookiesDelegate,
        IUpdateCookiesDelegate
    {
        // ...
    }
}
