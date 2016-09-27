using System.Threading.Tasks;

using Interfaces.Settings;

namespace GOG.Interfaces.Authorization
{
    public interface IAuthorizeDelegate
    {
        Task Authorize(IAuthenticationProperties usernamePassword);
    }

    public interface IIsAuthorizedDelegate
    {
        Task<bool> IsAuthorized();
    }

    public interface IDeauthorizeDelegate
    {
        Task Deauthorize();
    }

    public interface IAuthorizationController:
        IIsAuthorizedDelegate,
        IAuthorizeDelegate,
        IDeauthorizeDelegate
    {
        // ...
    }
}
