using System.Threading.Tasks;

using Interfaces.Status;

namespace GOG.Interfaces.Authorization
{
    public interface IAuthorizeDelegate
    {
        Task Authorize(string username, string password, IStatus status);
    }

    public interface IIsAuthorizedDelegate
    {
        Task<bool> IsAuthorized(IStatus status);
    }

    public interface IDeauthorizeDelegate
    {
        Task Deauthorize(IStatus status);
    }

    public interface IAuthorizationController:
        IIsAuthorizedDelegate,
        IAuthorizeDelegate,
        IDeauthorizeDelegate
    {
        // ...
    }
}
