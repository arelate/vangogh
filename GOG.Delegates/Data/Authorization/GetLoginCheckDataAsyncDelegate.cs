using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.QueryParameters;
using Models.Uris;
using Attributes;
using Delegates.Data.Console;
using Delegates.Data.Network;
using Delegates.Activities;
using Delegates.Conversions.Uris;
using Delegates.Itemizations.HtmlAttributes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Data.Authorization
{
    public class
        GetLoginCheckDataAsyncDelegate : IGetDataAsyncDelegate<string, (string AuthResponse, string Username, string
            Password)>
    {
        private const string pleaseEnterUsername = "Please enter your GOG.com username (email):";
        private const string pleaseEnterPassword = "Please enter password for {0}:";

        private IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate;
        private IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate;
        private IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate;

        private readonly IGetDataDelegate<string> getLineDataDelegate;
        private readonly IGetDataDelegate<string> getPrivateLineDataDelegate;

        private readonly IConvertDelegate<IDictionary<string, string>, string>
            convertDictionaryParametersToStringDelegate;

        private readonly IPostDataAsyncDelegate<string> postUriDataAsyncDelegate;

        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GetLineDataDelegate),
            typeof(GetPrivateLineDataDelegate),
            typeof(ItemizeLoginTokenAttributeValuesDelegate),
            typeof(ItemizeLoginIdAttributeValuesDelegate),
            typeof(ItemizeLoginUsernameAttributeValuesDelegate),
            typeof(ConvertDictionaryParametersToStringDelegate),
            typeof(PostUriDataAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public GetLoginCheckDataAsyncDelegate(
            IGetDataDelegate<string> getLineDataDelegate,
            IGetDataDelegate<string> getPrivateLineDataDelegate,
            IItemizeDelegate<string, string> itemizeLoginTokenAttribueValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginIdAttributeValueDelegate,
            IItemizeDelegate<string, string> itemizeLoginUsernameAttributeValueDelegate,
            IConvertDelegate<IDictionary<string, string>, string> convertDictionaryParametersToStringDelegate,
            IPostDataAsyncDelegate<string> postUriDataAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getLineDataDelegate = getLineDataDelegate;
            this.getPrivateLineDataDelegate = getPrivateLineDataDelegate;
            this.itemizeLoginTokenAttribueValueDelegate = itemizeLoginTokenAttribueValueDelegate;
            this.itemizeLoginIdAttributeValueDelegate = itemizeLoginIdAttributeValueDelegate;
            this.itemizeLoginUsernameAttributeValueDelegate = itemizeLoginUsernameAttributeValueDelegate;
            this.convertDictionaryParametersToStringDelegate = convertDictionaryParametersToStringDelegate;
            this.postUriDataAsyncDelegate = postUriDataAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<string> GetDataAsync((string AuthResponse, string Username, string Password) loginRequestData)
        {
            startDelegate.Start("Get login check result");

            var loginToken = itemizeLoginTokenAttribueValueDelegate.Itemize(loginRequestData.AuthResponse).First();

            // login using username / password or login id / password
            var loginUri = string.Empty;
            if (loginRequestData.AuthResponse.Contains(QueryParameters.LoginId))
            {
                var loginId = itemizeLoginIdAttributeValueDelegate.Itemize(loginRequestData.AuthResponse).First();
                QueryParametersCollections.LoginAuthenticate.Remove(QueryParameters.LoginUsername);
                QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginId] = loginId;
                loginUri = Uris.Endpoints.Authentication.Login;

                loginRequestData.Username = itemizeLoginUsernameAttributeValueDelegate
                    .Itemize(loginRequestData.AuthResponse).First();
            }
            else
            {
                QueryParametersCollections.LoginAuthenticate.Remove(QueryParameters.LoginId);
                loginUri = Uris.Endpoints.Authentication.LoginCheck;
            }

            if (string.IsNullOrEmpty(loginRequestData.Username))
                loginRequestData.Username = getLineDataDelegate.GetData(pleaseEnterUsername);

            if (string.IsNullOrEmpty(loginRequestData.Password))
                loginRequestData.Password = getPrivateLineDataDelegate.GetData(
                    string.Format(pleaseEnterPassword, loginRequestData.Username));

            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginUsername] = loginRequestData.Username;
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginPassword] = loginRequestData.Password;
            QueryParametersCollections.LoginAuthenticate[QueryParameters.LoginToken] = loginToken;

            var loginData = convertDictionaryParametersToStringDelegate.Convert(
                QueryParametersCollections.LoginAuthenticate);

            var loginCheckResult = await postUriDataAsyncDelegate.PostDataAsync(loginUri, loginData);

            completeDelegate.Complete();

            return loginCheckResult;
        }
    }
}