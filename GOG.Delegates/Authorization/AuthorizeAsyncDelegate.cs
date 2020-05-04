using System.Security;
using System.Threading.Tasks;
using Attributes;
using Delegates.Activities;
using GOG.Delegates.Confirmations.Authorization;
using GOG.Delegates.Data.Authorization;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Authorization;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Data;
using Models.QueryParameters;
using Models.Uris;

namespace GOG.Delegates.Authorization
{
    public class AuthorizeAsyncDelegate : IAuthorizeAsyncDelegate
    {
        private const string failedToAuthenticate = "Failed to authenticate user with provided username and password.";

        private const string recaptchaDetected = "Login page contains reCAPTCHA.\n" +
                                                 "Please login in the browser, then export the galaxy-login-* cookies into ./cookies.json\n" +
                                                 "{INSTRUCTIONS}";

        private readonly IConfirmAsyncDelegate<string> confirmUserIsLoggedInAsyncDelegate;
        private readonly IConfirmDelegate<string> confirmSuccessfulAuthorizationDelegate;
        private readonly IGetDataAsyncDelegate<string, string> getAuthenticationTokenDataAsyncDelegate;
        private readonly IGetDataAsyncDelegate<string, (string AuthResponse, string Username, string Password)>
            getLoginCheckDataAsyncDelegate;

        private readonly IGetDataAsyncDelegate<string, string> getTwoStepLoginCheckDataAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(ConfirmUserIsLoggedInAsyncDelegate),
            typeof(ConfirmSuccessfulAuthorizationDelegate),
            typeof(GetAuthenticationTokenDataAsyncDelegate),
            typeof(GetLoginCheckDataAsyncDelegate),
            typeof(GetTwoStepLoginCheckDataAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public AuthorizeAsyncDelegate(
            IConfirmAsyncDelegate<string> confirmUserIsLoggedInAsyncDelegate,
            IConfirmDelegate<string> confirmSuccessfulAuthorizationDelegate,
            IGetDataAsyncDelegate<string, string> getAuthenticationTokenDataAsyncDelegate,
            IGetDataAsyncDelegate<string, (string AuthResponse, string Username, string Password)>
                getLoginCheckDataAsyncDelegate,
            IGetDataAsyncDelegate<string, string> getTwoStepLoginCheckDataAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.confirmUserIsLoggedInAsyncDelegate = confirmUserIsLoggedInAsyncDelegate;
            this.confirmSuccessfulAuthorizationDelegate = confirmSuccessfulAuthorizationDelegate;
            
            this.getAuthenticationTokenDataAsyncDelegate = getAuthenticationTokenDataAsyncDelegate;
            this.getLoginCheckDataAsyncDelegate = getLoginCheckDataAsyncDelegate;
            this.getTwoStepLoginCheckDataAsyncDelegate = getTwoStepLoginCheckDataAsyncDelegate;
            
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task AuthorizeAsync(string username, string password)
        {
            // GOG.com quirk
            // Since introducing cookies support - it's expected that users need to authorize rarely.
            // Still, Authorization controller should support current authorization methods on GOG.com:
            // - Username (e-mail) and Password
            // - Two factor authentication
            // - We can also detect CAPTCHA and inform users what to do - this is typical for sales periods
            //   where it seems GOG.com tries to limit automated tools impact on the site

            startDelegate.Start("Authorize on GOG.com");

            if (await confirmUserIsLoggedInAsyncDelegate.ConfirmAsync(string.Empty))
            {
                // await statusController.InformAsync(authorizeTask, "User has already been logged in");
                completeDelegate.Complete();
                return;
            }

            var authResponse = await getAuthenticationTokenDataAsyncDelegate.GetDataAsync(string.Empty);

            if (authResponse.Contains(Uris.Roots.GoogleRecaptcha))
                throw new SecurityException(recaptchaDetected);

            var loginCheckResponse = await getLoginCheckDataAsyncDelegate.GetDataAsync(
                (authResponse, username, password));

            if (confirmSuccessfulAuthorizationDelegate.Confirm(loginCheckResponse))
            {
                completeDelegate.Complete();
                return;
            }

            if (!loginCheckResponse.Contains(QueryParameters.SecondStepAuthenticationToken))
                throw new SecurityException(failedToAuthenticate);

            var twoStepLoginCheckResponse = 
                await getTwoStepLoginCheckDataAsyncDelegate.GetDataAsync(loginCheckResponse);

            if (confirmSuccessfulAuthorizationDelegate.Confirm(twoStepLoginCheckResponse))
            {
                completeDelegate.Complete();
                return;
            }

            throw new SecurityException(failedToAuthenticate);
        }
    }
}