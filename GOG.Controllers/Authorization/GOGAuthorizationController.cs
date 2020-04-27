using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;
using Models.Uris;
using Models.QueryParameters;
using GOG.Interfaces.Controllers.Authorization;
using GOG.Models;

namespace GOG.Controllers.Authorization
{
    public class GOGAuthorizationController : IAuthorizationController
    {
        private const string failedToAuthenticate = "Failed to authenticate user with provided username and password.";
        private const string successfullyAuthorized = "Successfully authorized";

        private const string recaptchaDetected = "Login page contains reCAPTCHA.\n" +
                                                 "Please login in the browser, then export the galaxy-login-* cookies into ./cookies.json\n" +
                                                 "{INSTRUCTIONS}";

        private const string securityCodeHasBeenSent =
            "Enter four digits security code that has been sent to your email:";

        private const string pleaseEnterUsername = "Please enter your GOG.com username (email):";
        private const string pleaseEnterPassword = "Please enter password for {0}:";
        private const string gogData = "gogData";

        private readonly IGetDataDelegate<string> getLineDataDelegate;
        private readonly IGetDataDelegate<string> getPrivateLineDataDelegate;

        private readonly IConvertDelegate<IDictionary<string, string>, string>
            convertDictionaryParametersToStringDelegate;

        private readonly IConvertDelegate<(string, IDictionary<string, string>), string>
            convertUriParametersToUriDelegate;

        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;
        private readonly IPostDataAsyncDelegate<string> postUriDataAsyncDelegate;

        private readonly IConvertDelegate<string, UserData> convertJSONToUserDataDelegate;

        // IDictionary<string, IItemizeDelegate<string, string>> attributeValuesItemizeDelegates;
        private IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate;
        private IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate;
        private IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate;
        private IItemizeDelegate<string, string> itemizeSecondStepAuthenticationTokenAttributeValueDelegate;

        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "Delegates.Data.Console.GetLineDataDelegate,Delegates",
            "Delegates.Data.Console.GetPrivateLineDataDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeLoginTokenAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeLoginIdAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeLoginUsernameAttributeValuesDelegate,Delegates",
            "Delegates.Itemize.HtmlAttributes.ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate,Delegates",
            "Delegates.Convert.Uri.ConvertDictionaryParametersToStringDelegate,Delegates",
            "Delegates.Convert.Network.ConvertUriDictionaryParametersToUriDelegate,Delegates",
            "Delegates.Data.Network.GetUriDataAsyncDelegate,Delegates",
            "Delegates.Data.Network.PostUriDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToUserDataDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public GOGAuthorizationController(
            IGetDataDelegate<string> getLineDataDelegate,
            IGetDataDelegate<string> getPrivateLineDataDelegate,
            IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate,
            IItemizeDelegate<string, string> itemizeSecondStepAuthenticationTokenAttributeValueDelegate,
            IConvertDelegate<IDictionary<string, string>, string> convertDictionaryParametersToStringDelegate,
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IPostDataAsyncDelegate<string> postUriDataAsyncDelegate,
            IConvertDelegate<string, UserData> convertJSONToUserDataDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getLineDataDelegate = getLineDataDelegate;
            this.getPrivateLineDataDelegate = getPrivateLineDataDelegate;
            this.itemizeLoginTokenAttribueValueDelegate = itemizeLoginTokenAttribueValueDelegate;
            this.itemizeLoginIdAttributeValueDelegate = itemizeLoginIdAttributeValueDelegate;
            this.itemizeLoginUsernameAttributeValueDelegate = itemizeLoginUsernameAttributeValueDelegate;
            this.itemizeSecondStepAuthenticationTokenAttributeValueDelegate =
                itemizeSecondStepAuthenticationTokenAttributeValueDelegate;
            this.convertDictionaryParametersToStringDelegate = convertDictionaryParametersToStringDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.postUriDataAsyncDelegate = postUriDataAsyncDelegate;
            this.convertJSONToUserDataDelegate = convertJSONToUserDataDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<bool> AuthorizedAsync()
        {
            startDelegate.Start("Get userData.json");

            var userDataString = await getUriDataAsyncDelegate.GetDataAsync(
                Uris.Endpoints.Authentication.UserData);

            if (string.IsNullOrEmpty(userDataString)) return false;

            var userData = convertJSONToUserDataDelegate.Convert(userDataString);

            completeDelegate.Complete();

            return userData.IsLoggedIn;
        }

        public async Task<string> GetAuthenticationTokenResponseAsync()
        {
            startDelegate.Start("Get authorization token response");

            var uriParameters = convertUriParametersToUriDelegate.Convert((
                Uris.Endpoints.Authentication.Auth,
                QueryParametersCollections.Authenticate));
            // request authorization token
            var authResponse = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);

            completeDelegate.Complete();

            return authResponse;
        }

        public async Task<string> GetLoginCheckResponseAsync(string authResponse, string username, string password)
        {
            startDelegate.Start("Get login check result");

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

            if (string.IsNullOrEmpty(username))
                username = getLineDataDelegate.GetData(pleaseEnterUsername);

            if (string.IsNullOrEmpty(password))
                password = getPrivateLineDataDelegate.GetData(
                    string.Format(pleaseEnterPassword, username));

            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginUsername] = username;
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginPassword] = password;
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginToken] = loginToken;

            var loginData = convertDictionaryParametersToStringDelegate.Convert(
                QueryParametersCollections.LoginAuthenticate);

            var loginCheckResult = await postUriDataAsyncDelegate.PostDataAsync(loginUri, loginData);

            completeDelegate.Complete();

            return loginCheckResult;
        }

        public async Task<string> GetTwoStepLoginCheckResponseAsync(string loginCheckResult)
        {
            startDelegate.Start("Get second step authentication result");

            // 2FA is enabled for this user - ask for the code
            var securityCode = getPrivateLineDataDelegate.GetData(securityCodeHasBeenSent);

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

            completeDelegate.Complete();

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
                completeDelegate.Complete();
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

            startDelegate.Start("Authorize on GOG.com");

            if (await AuthorizedAsync())
            {
                // await statusController.InformAsync(authorizeTask, "User has already been logged in");
                completeDelegate.Complete();
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