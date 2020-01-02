using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Correct;

using Interfaces.Controllers.Uri;
using Interfaces.Controllers.Network;
using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Itemize;

using Attributes;

using Models.Uris;
using Models.QueryParameters;
using Models.Dependencies;

using GOG.Interfaces.Controllers.Authorization;

namespace GOG.Controllers.Authorization
{
    public class GOGAuthorizationController : IAuthorizationController
    {
        const string failedToAuthenticate = "Failed to authenticate user with provided username and password.";
        const string successfullyAuthorized = "Successfully authorized";
        const string recaptchaDetected = "Login page contains reCAPTCHA.\n" +
            "Please login in the browser, then export the galaxy-login-* cookies into ./cookies.json\n" +
            "{INSTRUCTIONS}";
        const string gogData = "gogData";

        ICorrectAsyncDelegate<string[]> correctUsernamePasswordAsyncDelegate;
        ICorrectAsyncDelegate<string> correctSecurityCodeAsyncDelegate;
        IUriController uriController;
        INetworkController networkController;
        ISerializationController<string> serializationController;
        // IDictionary<string, IItemizeDelegate<string, string>> attributeValuesItemizeDelegates;
        IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate;
        IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate;
        IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate;
        IItemizeDelegate<string, string> itemizeSecondStepAuthenticationTokenAttributeValueDelegate;

        readonly IActionLogController actionLogController;

        [Dependencies(
            "Delegates.Correct.CorrectUsernamePasswordAsyncDelegate,Delegates",
            "Delegates.Correct.CorrectSecurityCodeAsyncDelegate,Delegates",
            "Delegates.Itemize.Attributes.ItemizeLoginTokenAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.Attributes.ItemizeLoginIdAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.Attributes.ItemizeLoginUsernameAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.Attributes.ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate,Delegates",
            "Controllers.Uri.UriController,Controllers",
            "Controllers.Network.NetworkController,Controllers",
            Dependencies.JSONSerializationController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public GOGAuthorizationController(
            ICorrectAsyncDelegate<string[]> correctUsernamePasswordAsyncDelegate,
            ICorrectAsyncDelegate<string> correctSecurityCodeAsyncDelegate,
            IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate,
            IItemizeDelegate<string, string> itemizeSecondStepAuthenticationTokenAttributeValueDelegate,
            IUriController uriController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            // IDictionary<string, IItemizeDelegate<string, string>> attributeValuesItemizeDelegates,
            IActionLogController actionLogController)
        {
            this.correctUsernamePasswordAsyncDelegate = correctUsernamePasswordAsyncDelegate;
            this.correctSecurityCodeAsyncDelegate = correctSecurityCodeAsyncDelegate;
            this.itemizeLoginTokenAttribueValueDelegate = itemizeLoginTokenAttribueValueDelegate;
            this.itemizeLoginIdAttributeValueDelegate = itemizeLoginIdAttributeValueDelegate;
            this.itemizeLoginUsernameAttributeValueDelegate = itemizeLoginUsernameAttributeValueDelegate;
            this.itemizeSecondStepAuthenticationTokenAttributeValueDelegate = itemizeSecondStepAuthenticationTokenAttributeValueDelegate;
            this.uriController = uriController;
            this.networkController = networkController;
            this.serializationController = serializationController;
            // this.attributeValuesItemizeDelegates = attributeValuesItemizeDelegates;
            this.actionLogController = actionLogController;
        }

        public async Task<bool> AuthorizedAsync()
        {
            actionLogController.StartAction("Get userData.json");

            var userDataString = await networkController.GetResourceAsync(
                Uris.Endpoints.Authentication.UserData);

            if (string.IsNullOrEmpty(userDataString)) return false;

            var userData = serializationController.Deserialize<Models.UserData>(userDataString);

            actionLogController.CompleteAction();

            return userData.IsLoggedIn;
        }

        public async Task<string> GetAuthenticationTokenResponseAsync()
        {
            actionLogController.StartAction("Get authorization token response");

            // request authorization token
            var authResponse = await networkController.GetResourceAsync(
                Uris.Endpoints.Authentication.Auth,
                QueryParametersCollections.Authenticate);

            actionLogController.CompleteAction();

            return authResponse;
        }

        public async Task<string> GetLoginCheckResponseAsync(string authResponse, string username, string password)
        {
            actionLogController.StartAction("Get login check result");

            var loginToken = itemizeLoginTokenAttribueValueDelegate.Itemize(authResponse).First();

            // login using username / password or login id / password
            var loginUri = string.Empty;
            if (authResponse.Contains(QueryParameters.LoginId))
            {
                var loginId = itemizeLoginIdAttributeValueDelegate.Itemize(authResponse).First();
                QueryParametersCollections.LoginAuthenticate.Remove(QueryParameters.LoginUsername);
                QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginId] = loginId;
                loginUri = Uris.Endpoints.Authentication.Login;

                username = itemizeLoginUsernameAttributeValueDelegate.Itemize(authResponse).First();
            }
            else
            {
                QueryParametersCollections.LoginAuthenticate.Remove(QueryParameters.LoginId);
                loginUri = Uris.Endpoints.Authentication.LoginCheck;
            }

            var usernamePassword = await correctUsernamePasswordAsyncDelegate.CorrectAsync(
                new string[] { username, password });

            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginUsername] = usernamePassword[0];
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginPassword] = usernamePassword[1];
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginToken] = loginToken;

            var loginData = uriController.ConcatenateQueryParameters(QueryParametersCollections.LoginAuthenticate);

            var loginCheckResult = await networkController.PostDataToResourceAsync(loginUri, null, loginData);

            actionLogController.CompleteAction();

            return loginCheckResult;
        }

        public async Task<string> GetTwoStepLoginCheckResponseAsync(string loginCheckResult)
        {
            actionLogController.StartAction("Get second step authentication result");

            // 2FA is enabled for this user - ask for the code
            var securityCode = await correctSecurityCodeAsyncDelegate.CorrectAsync(null);

            var secondStepAuthenticationToken = 
                itemizeSecondStepAuthenticationTokenAttributeValueDelegate.Itemize(
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
                QueryParameters.SecondStepAuthenticationToken] = secondStepAuthenticationToken;

            var secondStepData = uriController.ConcatenateQueryParameters(
                QueryParametersCollections.SecondStepAuthentication);

            var secondStepLoginCheckResult = await networkController.PostDataToResourceAsync(
                Uris.Endpoints.Authentication.TwoStep, null, secondStepData);

            actionLogController.CompleteAction();

            return secondStepLoginCheckResult;
        }

        public async Task ThrowSecurityExceptionAsync(string message)
        {
            // await statusController.FailAsync(status, message);
            throw new System.Security.SecurityException(message);
        }

        public async Task<bool> CheckAuthorizationSuccessAsync(string response)
        {
            if (response.Contains(gogData))
            {
                // await statusController.InformAsync(status, successfullyAuthorized);
                actionLogController.CompleteAction();
                return true;
            }

            return false;
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

            actionLogController.StartAction("Authorize on GOG.com");

            if (await AuthorizedAsync())
            {
                // await statusController.InformAsync(authorizeTask, "User has already been logged in");
                actionLogController.CompleteAction();
                return;
            }

            var authResponse = await GetAuthenticationTokenResponseAsync();

            if (authResponse.Contains(Uris.Roots.GoogleRecaptcha))
                await ThrowSecurityExceptionAsync(recaptchaDetected);

            var loginCheckResponse = await GetLoginCheckResponseAsync(authResponse, username, password);

            if (await CheckAuthorizationSuccessAsync(loginCheckResponse)) return;

            if (!loginCheckResponse.Contains(QueryParameters.SecondStepAuthenticationToken))
                await ThrowSecurityExceptionAsync(failedToAuthenticate);

            var twoStepLoginCheckResponse = await GetTwoStepLoginCheckResponseAsync(loginCheckResponse);

            if (await CheckAuthorizationSuccessAsync(twoStepLoginCheckResponse)) return;

            await ThrowSecurityExceptionAsync(failedToAuthenticate);
        }
    }
}
