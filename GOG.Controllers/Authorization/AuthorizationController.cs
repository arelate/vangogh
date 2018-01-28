using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Correct;

using Interfaces.Controllers.Uri;
using Interfaces.Controllers.Network;
using Interfaces.Controllers.Serialization;

using Interfaces.Extraction;
using Interfaces.Status;

using Models.Uris;
using Models.QueryParameters;

using GOG.Interfaces.Controllers.Authorization;

namespace GOG.Controllers.Authorization
{
    public class AuthorizationController : IAuthorizationController
    {
        private const string failedToAuthenticate = "Failed to authenticate user with provided username and password.";
        private const string successfullyAuthorized = "Successfully authorized";
        private const string recaptchaDetected = "Login page contains reCAPTCHA.\n" +
            "Please login in the browser, then export the galaxy-login-* cookies into ./cookies.json\n" +
            "{INSTRUCTIONS}";
        private const string gogData = "gogData";

        private ICorrectAsyncDelegate<string[]> correctUsernamePasswordAsyncDelegate;
        private ICorrectAsyncDelegate<string> correctSecurityCodeAsyncDelegate;
        private IUriController uriController;
        private INetworkController networkController;
        private ISerializationController<string> serializationController;
        private IDictionary<string, IStringExtractionController> extractionControllers;
        private IStatusController statusController;

        public AuthorizationController(
            ICorrectAsyncDelegate<string[]> correctUsernamePasswordAsyncDelegate,
            ICorrectAsyncDelegate<string> correctSecurityCodeAsyncDelegate,
            IUriController uriController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IDictionary<string, IStringExtractionController> extractionControllers,
            IStatusController statusController)
        {
            this.correctUsernamePasswordAsyncDelegate = correctUsernamePasswordAsyncDelegate;
            this.correctSecurityCodeAsyncDelegate = correctSecurityCodeAsyncDelegate;
            this.uriController = uriController;
            this.networkController = networkController;
            this.serializationController = serializationController;
            this.extractionControllers = extractionControllers;
            this.statusController = statusController;
        }

        public async Task<bool> AuthorizedAsync(IStatus status)
        {
            var getUserDataTask = await statusController.CreateAsync(status, "Get userData.json");

            var userDataString = await networkController.GetResourceAsync(getUserDataTask, Uris.Paths.Authentication.UserData);
            if (string.IsNullOrEmpty(userDataString)) return false;

            var userData = serializationController.Deserialize<Models.UserData>(userDataString);

            await statusController.CompleteAsync(getUserDataTask);

            return userData.IsLoggedIn;
        }

        public async Task<string> GetAuthenticationTokenResponseAsync(IStatus status)
        {
            var getAuthenticationTokenResponseTask = await statusController.CreateAsync(status, "Get authorization token response");

            // request authorization token
            var authResponse = await networkController.GetResourceAsync(
                status,
                Uris.Paths.Authentication.Auth,
                QueryParametersCollections.Authenticate);

            await statusController.CompleteAsync(getAuthenticationTokenResponseTask);

            return authResponse;
        }

        public async Task<string> GetLoginCheckResponseAsync(string authResponse, string username, string password, IStatus status)
        {
            var getLoginCheckResponseTask = await statusController.CreateAsync(status, "Get login check result");

            var loginToken = extractionControllers[
                QueryParameters.LoginUnderscoreToken].ExtractMultiple(
                authResponse).First();

            // login using username / password or login id / password
            var loginUri = string.Empty;
            if (authResponse.Contains(QueryParameters.LoginId))
            {
                var loginId = extractionControllers[
                    QueryParameters.LoginId].ExtractMultiple(
                    authResponse).First();
                QueryParametersCollections.LoginAuthenticate.Remove(QueryParameters.LoginUsername);
                QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginId] = loginId;
                loginUri = Uris.Paths.Authentication.Login;

                username = extractionControllers[
                    QueryParameters.LoginUsername].ExtractMultiple(
                    authResponse).First();
            }
            else
            {
                QueryParametersCollections.LoginAuthenticate.Remove(QueryParameters.LoginId);
                loginUri = Uris.Paths.Authentication.LoginCheck;
            }

            var usernamePassword = await correctUsernamePasswordAsyncDelegate.CorrectAsync(
                new string[] { username, password }, 
                getLoginCheckResponseTask);

            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginUsername] = usernamePassword[0];
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginPassword] = usernamePassword[1];
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginUnderscoreToken] = loginToken;

            string loginData = uriController.ConcatenateQueryParameters(QueryParametersCollections.LoginAuthenticate);

            var loginCheckResult = await networkController.PostDataToResourceAsync(getLoginCheckResponseTask, loginUri, null, loginData);

            await statusController.CompleteAsync(getLoginCheckResponseTask);

            return loginCheckResult;
        }

        public async Task<string> GetTwoStepLoginCheckResponseAsync(string loginCheckResult, IStatus status)
        {
            var getTwoStepLoginCheckResponseTask = await statusController.CreateAsync(status, "Get second step authentication result");

            // 2FA is enabled for this user - ask for the code
            var securityCode = await correctSecurityCodeAsyncDelegate.CorrectAsync(null, getTwoStepLoginCheckResponseTask);

            var secondStepAuthenticationToken = extractionControllers[
                QueryParameters.SecondStepAuthenticationUnderscoreToken].ExtractMultiple(
                loginCheckResult).First();

            QueryParametersCollections.SecondStepAuthentication[
                QueryParameters.SecondStepAuthenticationTokenLetter1] = securityCode[0].ToString();
            QueryParametersCollections.SecondStepAuthentication[
                QueryParameters.SecondStepAuthenticationTokenLetter2] = securityCode[1].ToString();
            QueryParametersCollections.SecondStepAuthentication[
                QueryParameters.SecondStepAuthenticationTokenLetter3] = securityCode[2].ToString();
            QueryParametersCollections.SecondStepAuthentication[
                QueryParameters.SecondStepAuthenticationTokenLetter4] = securityCode[3].ToString();
            QueryParametersCollections.SecondStepAuthentication[
                QueryParameters.SecondStepAuthenticationUnderscoreToken] = secondStepAuthenticationToken;

            string secondStepData = uriController.ConcatenateQueryParameters(QueryParametersCollections.SecondStepAuthentication);

            var secondStepLoginCheckResult = await networkController.PostDataToResourceAsync(status, Uris.Paths.Authentication.TwoStep, null, secondStepData);

            await statusController.CompleteAsync(getTwoStepLoginCheckResponseTask);

            return secondStepLoginCheckResult;
        }

        public async Task ThrowSecurityExceptionAsync(IStatus status, string message)
        {
            await statusController.FailAsync(status, message);
            await statusController.CompleteAsync(status);
            throw new System.Security.SecurityException(message);
        }

        public async Task<bool> CheckAuthorizationSuccessAsync(string response, IStatus status)
        {
            if (response.Contains(gogData))
            {
                await statusController.InformAsync(status, successfullyAuthorized);
                await statusController.CompleteAsync(status);
                return true;
            }

            return false;
        }

        public async Task AuthorizeAsync(string username, string password, IStatus status)
        {
            // GOG.com quirk
            // Since introducing cookies support - it's expected that users need to authorize rarely.
            // Still, Authorization controller should support current authorization methods on GOG.com:
            // - Username (e-mail) and Password
            // - Two factor authentication
            // - We can also detect CAPTCHA and inform users what to do - this is typical for sales periods
            //   where it seems GOG.com tries to limit automated tools impact on the site

            var authorizeTask = await statusController.CreateAsync(status, "Authorize on GOG.com");

            if (await AuthorizedAsync(status))
            {
                await statusController.InformAsync(authorizeTask, "User has already been logged in");
                await statusController.CompleteAsync(authorizeTask);
                return;
            }

            var authResponse = await GetAuthenticationTokenResponseAsync(authorizeTask);

            if (authResponse.Contains(Uris.Roots.GoogleRecaptcha))
                await ThrowSecurityExceptionAsync(authorizeTask, recaptchaDetected);

            var loginCheckResponse = await GetLoginCheckResponseAsync(authResponse, username, password, authorizeTask);

            if (await CheckAuthorizationSuccessAsync(loginCheckResponse, authorizeTask)) return;

            if (!loginCheckResponse.Contains(QueryParameters.SecondStepAuthenticationUnderscoreToken))
                await ThrowSecurityExceptionAsync(authorizeTask, failedToAuthenticate);

            var twoStepLoginCheckResponse = await GetTwoStepLoginCheckResponseAsync(loginCheckResponse, authorizeTask);

            if (await CheckAuthorizationSuccessAsync(twoStepLoginCheckResponse, authorizeTask)) return;

            await ThrowSecurityExceptionAsync(authorizeTask, failedToAuthenticate);
        }
    }
}
