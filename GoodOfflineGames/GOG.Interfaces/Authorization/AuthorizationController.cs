using System.Threading.Tasks;

namespace GOG.Interfaces.Authorization
{
    public interface IAuthorizeDelegate
    {
        Task Authorize(string username, string password);
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
