using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface IAuthorizeDelegate
    {
        Task<bool> Authorize(ICredentials credentials);
    }

    public interface IDeauthorizeDelegate
    {
        Task Deauthorize();
    }

    public interface IAuthorizationController:
        IAuthorizeDelegate,
        IDeauthorizeDelegate
    {
        // ...
    }
}
