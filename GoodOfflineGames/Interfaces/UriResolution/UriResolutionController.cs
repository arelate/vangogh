using System.Threading.Tasks;

namespace Interfaces.UriResolution
{
    public interface IResolveUriDelegate
    {
        Task<string> ResolveUri(string uri);
    }

    public interface IUriResolutionController :
        IResolveUriDelegate
    {
        // ...
    }
}
