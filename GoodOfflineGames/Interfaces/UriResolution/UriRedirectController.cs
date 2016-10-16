using System.Threading.Tasks;

namespace Interfaces.UriRedirection
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
