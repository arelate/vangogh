using System.Threading.Tasks;

namespace Interfaces.Cookies
{
    public interface IGetCookiesDelegate
    {
        Task<string[]> GetCookies();
    }

    public interface ICookiesController:
        IGetCookiesDelegate
    {
        // ...
    }
}
