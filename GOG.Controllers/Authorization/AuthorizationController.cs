using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using GOG.Interfaces.Authorization;

using Interfaces.Uri;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.PropertyValidation;
using Interfaces.Status;

using Models.Uris;
using Models.QueryParameters;

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

        private IValidatePropertiesDelegate<string> usernamePasswordValidationDelegate;
        private IValidatePropertiesDelegate<string> securityCodeValidationDelegate;
        private IUriController uriController;
        private INetworkController networkController;
        private ISerializationController<string> serializationController;
        private IDictionary<string, IStringExtractionController> extractionControllers;
        private IStatusController statusController;

        public AuthorizationController(
            IValidatePropertiesDelegate<string> usernamePasswordValidationDelegate,
            IValidatePropertiesDelegate<string> securityCodeValidationDelegate,
            IUriController uriController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IDictionary<string, IStringExtractionController> extractionControllers,
            IStatusController statusController)
        {
            this.usernamePasswordValidationDelegate = usernamePasswordValidationDelegate;
            this.securityCodeValidationDelegate = securityCodeValidationDelegate;
            this.uriController = uriController;
            this.networkController = networkController;
            this.serializationController = serializationController;
            this.extractionControllers = extractionControllers;
            this.statusController = statusController;
        }

        public async Task<bool> IsAuthorized(IStatus status)
        {
            var getUserDataTask = statusController.Create(status, "Get userData.json");

            var userDataString = await networkController.Get(getUserDataTask, Uris.Paths.Authentication.UserData);
            if (string.IsNullOrEmpty(userDataString)) return false;

            var userData = serializationController.Deserialize<Models.UserData>(userDataString);

            statusController.Complete(getUserDataTask);

            return userData.IsLoggedIn;
        }

        public async Task<string> GetAuthenticationTokenResponse(IStatus status)
        {
            var getAuthenticationTokenResponseTask = statusController.Create(status, "Get authorization token response");

            // request authorization token
            var authResponse = await networkController.Get(
                status,
                Uris.Paths.Authentication.Auth,
                QueryParametersCollections.Authenticate);

            statusController.Complete(getAuthenticationTokenResponseTask);

            return authResponse;
        }

        public async Task<string> GetLoginCheckResponse(string authResponse, string username, string password, IStatus status)
        {
            var getLoginCheckResponseTask = statusController.Create(status, "Get login check result");

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

            var usernamePassword = usernamePasswordValidationDelegate.ValidateProperties(username, password);

            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginUsername] = usernamePassword[0];
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginPassword] = usernamePassword[1];
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginUnderscoreToken] = loginToken;

            string loginData = uriController.ConcatenateQueryParameters(QueryParametersCollections.LoginAuthenticate);

            var loginCheckResult = await networkController.Post(getLoginCheckResponseTask, loginUri, null, loginData);

            statusController.Complete(getLoginCheckResponseTask);

            return loginCheckResult;
        }

        public async Task<string> GetTwoStepLoginCheckResponse(string loginCheckResult, IStatus status)
        {
            var getTwoStepLoginCheckResponseTask = statusController.Create(status, "Get second step authentication result");

            // 2FA is enabled for this user - ask for the code
            var securityCode = securityCodeValidationDelegate.ValidateProperties(
                new string[0]).First();

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

            var secondStepLoginCheckResult = await networkController.Post(status, Uris.Paths.Authentication.TwoStep, null, secondStepData);

            statusController.Complete(getTwoStepLoginCheckResponseTask);

            return secondStepLoginCheckResult;
        }

        public void ThrowSecurityException(IStatus status, string message)
        {
            statusController.Fail(status, message);
            statusController.Complete(status);
            throw new System.Security.SecurityException(message);
        }

        public bool CheckAuthorizationSuccess(string response, IStatus status)
        {
            if (response.Contains(gogData))
            {
                statusController.Inform(status, successfullyAuthorized);
                statusController.Complete(status);
                return true;
            }

            return false;
        }

        public async Task Authorize(string username, string password, IStatus status)
        {
            // GOG.com quirk
            // Since introducing cookies support - it's expected that users need to authorize rarely.
            // Still, Authorization controller should support current authorization methods on GOG.com:
            // - Username (e-mail) and Password
            // - Two factor authentication
            // - We can also detect CAPTCHA and inform users what to do - this is typical for sales periods
            //   where it seems GOG.com tries to limit automated tools impact on the site

            var authorizeTask = statusController.Create(status, "Authorize on GOG.com");

            if (await IsAuthorized(status))
            {
                statusController.Inform(authorizeTask, "User has already been logged in");
                statusController.Complete(authorizeTask);
                return;
            }

            var authResponse = await GetAuthenticationTokenResponse(authorizeTask);

            if (authResponse.Contains(Uris.Roots.GoogleRecaptcha))
                ThrowSecurityException(authorizeTask, recaptchaDetected);

            var loginCheckResponse = await GetLoginCheckResponse(authResponse, username, password, authorizeTask);

            if (CheckAuthorizationSuccess(loginCheckResponse, authorizeTask)) return;

            if (!loginCheckResponse.Contains(QueryParameters.SecondStepAuthenticationUnderscoreToken))
                ThrowSecurityException(authorizeTask, failedToAuthenticate);

            var twoStepLoginCheckResponse = await GetTwoStepLoginCheckResponse(loginCheckResponse, authorizeTask);

            if (CheckAuthorizationSuccess(twoStepLoginCheckResponse, authorizeTask)) return;

            ThrowSecurityException(authorizeTask, failedToAuthenticate);
        }
    }
}
