using System.Threading.Tasks;

namespace Interfaces.UriRedirect
{
    public interface IGetUriRedirectDelegate
    {
        Task<string> GetUriRedirect(string uri);
    }

    public interface IUriRedirectController:
        IGetUriRedirectDelegate
    {
        // ...
    }
}
