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

    public interface IGetAuthenticationTokenResponseDelegate
    {
        Task<string> GetAuthenticationTokenResponse(IStatus status);
    }

    public interface IGetLoginCheckResponseDelegate
    {
        Task<string> GetLoginCheckResponse(string authResponse, string username, string password, IStatus status);
    }

    public interface IGetTwoStepLoginCheckResponseDelegate
    {
        Task<string> GetTwoStepLoginCheckResponse(string loginCheckResult, IStatus status);
    }

    public interface IAuthorizationController:
        IIsAuthorizedDelegate,
        IGetAuthenticationTokenResponseDelegate,
        IGetLoginCheckResponseDelegate,
        IGetTwoStepLoginCheckResponseDelegate,
        IAuthorizeDelegate
    {
        // ...
    }
}
