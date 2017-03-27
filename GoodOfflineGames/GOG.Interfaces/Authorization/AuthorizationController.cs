using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace GOG.Interfaces.Authorization
{
    public interface IAuthorizeDelegate
    {
        Task Authorize(string username, string password, ITaskStatus taskStatus);
    }

    public interface IIsAuthorizedDelegate
    {
        Task<bool> IsAuthorized(ITaskStatus taskStatus);
    }

    public interface IDeauthorizeDelegate
    {
        Task Deauthorize(ITaskStatus taskStatus);
    }

    public interface IAuthorizationController:
        IIsAuthorizedDelegate,
        IAuthorizeDelegate,
        IDeauthorizeDelegate
    {
        // ...
    }
}
