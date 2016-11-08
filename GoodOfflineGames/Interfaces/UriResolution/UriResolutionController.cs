using System.Threading.Tasks;

namespace Interfaces.UriResolution
{
    public interface IResolveUriDelegate
    {
        string ResolveUri(string uri);
    }

    public interface IUriResolutionController :
        IResolveUriDelegate
    {
        // ...
    }
}
