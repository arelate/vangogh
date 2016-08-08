using System.Threading.Tasks;

using Interfaces.Settings;

namespace GOG.Interfaces.Authorization
{
    public interface IAuthorizeDelegate
    {
        Task Authorize(IAuthenticateProperties usernamePassword);
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
