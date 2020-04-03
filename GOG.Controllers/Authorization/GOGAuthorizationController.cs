using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Correct;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Models.Uris;
using Models.QueryParameters;
using Models.Dependencies;

using GOG.Interfaces.Controllers.Authorization;

using GOG.Models;

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
        private readonly IConvertDelegate<IDictionary<string, string>, string> convertDictionaryParametersToStringDelegate;
        private readonly IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate;
        private readonly IGetDataAsyncDelegate<string> getUriDataAsyncDelegate;
        private readonly IPostDataAsyncDelegate<string> postUriDataAsyncDelegate;
        private readonly IConvertDelegate<string, UserData> convertJSONToUserDataDelegate;
        // IDictionary<string, IItemizeDelegate<string, string>> attributeValuesItemizeDelegates;
        IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate;
        IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate;
        IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate;
        IItemizeDelegate<string, string> itemizeSecondStepAuthenticationTokenAttributeValueDelegate;

        readonly IActionLogController actionLogController;

        [Dependencies(
            "Delegates.Correct.CorrectUsernamePasswordAsyncDelegate,Delegates",
            "Delegates.Correct.CorrectSecurityCodeAsyncDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeLoginTokenAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeLoginIdAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeLoginUsernameAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate,Delegates",
            "Delegates.Convert.Uri.ConvertDictionaryParametersToStringDelegate,Delegates",
            "Delegates.Convert.Network.ConvertUriDictionaryParametersToUriDelegate,Delegates",
            "Delegates.GetData.Network.GetUriDataAsyncDelegate,Delegates",
            "Delegates.PostData.Network.PostUriDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToUserDataDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GOGAuthorizationController(
            ICorrectAsyncDelegate<string[]> correctUsernamePasswordAsyncDelegate,
            ICorrectAsyncDelegate<string> correctSecurityCodeAsyncDelegate,
            IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate,
            IItemizeDelegate<string, string> itemizeSecondStepAuthenticationTokenAttributeValueDelegate,
            IConvertDelegate<IDictionary<string, string>, string> convertDictionaryParametersToStringDelegate,
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate,
            IPostDataAsyncDelegate<string> postUriDataAsyncDelegate,
            IConvertDelegate<string, UserData> convertJSONToUserDataDelegate,
            IActionLogController actionLogController)
        {
            this.correctUsernamePasswordAsyncDelegate = correctUsernamePasswordAsyncDelegate;
            this.correctSecurityCodeAsyncDelegate = correctSecurityCodeAsyncDelegate;
            this.itemizeLoginTokenAttribueValueDelegate = itemizeLoginTokenAttribueValueDelegate;
            this.itemizeLoginIdAttributeValueDelegate = itemizeLoginIdAttributeValueDelegate;
            this.itemizeLoginUsernameAttributeValueDelegate = itemizeLoginUsernameAttributeValueDelegate;
            this.itemizeSecondStepAuthenticationTokenAttributeValueDelegate = itemizeSecondStepAuthenticationTokenAttributeValueDelegate;
            this.convertDictionaryParametersToStringDelegate = convertDictionaryParametersToStringDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.postUriDataAsyncDelegate = postUriDataAsyncDelegate;
            this.convertJSONToUserDataDelegate = convertJSONToUserDataDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task<bool> AuthorizedAsync()
        {
            actionLogController.StartAction("Get userData.json");

            var userDataString = await getUriDataAsyncDelegate.GetDataAsync(
                Uris.Endpoints.Authentication.UserData);

            if (string.IsNullOrEmpty(userDataString)) return false;

            var userData = convertJSONToUserDataDelegate.Convert(userDataString);

            actionLogController.CompleteAction();

            return userData.IsLoggedIn;
        }

        public async Task<string> GetAuthenticationTokenResponseAsync()
        {
            actionLogController.StartAction("Get authorization token response");

            var uriParameters = convertUriParametersToUriDelegate.Convert((
                Uris.Endpoints.Authentication.Auth,
                QueryParametersCollections.Authenticate));
            // request authorization token
            var authResponse = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);

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

            var loginData = convertDictionaryParametersToStringDelegate.Convert(
                QueryParametersCollections.LoginAuthenticate);

            var loginCheckResult = await postUriDataAsyncDelegate.PostDataAsync(loginUri, loginData);

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

            var secondStepData = convertDictionaryParametersToStringDelegate.Convert(
                QueryParametersCollections.SecondStepAuthentication);

            var secondStepLoginCheckResult = await postUriDataAsyncDelegate.PostDataAsync(
                Uris.Endpoints.Authentication.TwoStep, 
                secondStepData);

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
