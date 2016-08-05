using System.Threading.Tasks;

using Interfaces.Settings;

namespace GOG.Interfaces
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
