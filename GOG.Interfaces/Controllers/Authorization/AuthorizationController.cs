using System.Threading.Tasks;

namespace GOG.Interfaces.Controllers.Authorization
{
    public interface IAuthorizeAsyncDelegate
    {
        Task AuthorizeAsync(string username, string password);
    }

    public interface IAuthorizedAsyncDelegate
    {
        Task<bool> AuthorizedAsync();
    }

    public interface IGetAuthenticationTokenResponseAsyncDelegate
    {
        Task<string> GetAuthenticationTokenResponseAsync();
    }

    public interface IGetLoginCheckResponseAsyncDelegate
    {
        Task<string> GetLoginCheckResponseAsync(string authResponse, string username, string password);
    }

    public interface IGetTwoStepLoginCheckResponseAsyncDelegate
    {
        Task<string> GetTwoStepLoginCheckResponseAsync(string loginCheckResult);
    }

    public interface ICheckAuthorizationSuccessAsyncDelegate
    {
        Task<bool> CheckAuthorizationSuccessAsync(string response);
    }

    public interface IThrowSecurityExceptionAsyncDelegate
    {
        Task ThrowSecurityExceptionAsync(string message);
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
