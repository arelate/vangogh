using System.Threading.Tasks;

using Interfaces.Status;

namespace GOG.Interfaces.Authorization
{
    public interface IAuthorizeAsyncDelegate
    {
        Task AuthorizeAsync(string username, string password, IStatus status);
    }

    public interface IAuthorizedAsyncDelegate
    {
        Task<bool> AuthorizedAsync(IStatus status);
    }

    public interface IGetAuthenticationTokenResponseAsyncDelegate
    {
        Task<string> GetAuthenticationTokenResponseAsync(IStatus status);
    }

    public interface IGetLoginCheckResponseAsyncDelegate
    {
        Task<string> GetLoginCheckResponseAsync(string authResponse, string username, string password, IStatus status);
    }

    public interface IGetTwoStepLoginCheckResponseAsyncDelegate
    {
        Task<string> GetTwoStepLoginCheckResponseAsync(string loginCheckResult, IStatus status);
    }

    public interface ICheckAuthorizationSuccessAsyncDelegate
    {
        Task<bool> CheckAuthorizationSuccessAsync(string response, IStatus status);
    }

    public interface IThrowSecurityExceptionAsyncDelegate
    {
        Task ThrowSecurityExceptionAsync(IStatus status, string message);
    }


    public interface IAuthorizationController:
        IAuthorizedAsyncDelegate,
        IGetAuthenticationTokenResponseAsyncDelegate,
        IGetLoginCheckResponseAsyncDelegate,
        IGetTwoStepLoginCheckResponseAsyncDelegate,
        ICheckAuthorizationSuccessAsyncDelegate,
        IThrowSecurityExceptionAsyncDelegate,
        IAuthorizeAsyncDelegate
    {
        // ...
    }
}
