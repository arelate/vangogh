using System.Threading.Tasks;
using System.Linq;

using GOG.Interfaces;

using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Console;
using Interfaces.Settings;

using Models.Uris;
using Models.QueryParameters;

namespace GOG.Controllers.Authorization
{
    public class AuthorizationController : IAuthorizationController
    {
        private const string authorizingOnGOG = "Authorizing {0} on GOG.com...";
        private const string successfullyAuthorizedOnGOG = "Successfully authorized {0} on GOG.com.";
        private const string failedToAuthenticate = "Failed to authenticate user with provided username and password.";
        private const string securityCodeHasBeenSent = "Enter security code that has been sent to {0}:";

        private IUriController uriController;
        private IStringNetworkController stringNetworkController;
        private IExtractionController extractionController;
        private IConsoleController consoleController;

        public AuthorizationController(
            IUriController uriController,
            IStringNetworkController stringNetworkController,
            IExtractionController extractionController,
            IConsoleController consoleController)
        {
            this.uriController = uriController;
            this.stringNetworkController = stringNetworkController;
            this.extractionController = extractionController;
            this.consoleController = consoleController;
        }

        public async Task Authorize(IAuthenticateProperties usernamePassword)
        {
            consoleController.WriteLine(authorizingOnGOG, MessageType.Progress, usernamePassword.Username);

            // request authorization token
            string authResponse = await stringNetworkController.GetString(Uris.Paths.Authentication.Auth, QueryParameters.Authenticate);

            string loginToken = extractionController.ExtractMultiple(authResponse).First();

            // login using username / password

            QueryParameters.LoginAuthenticate["login[username]"] = usernamePassword.Username;
            QueryParameters.LoginAuthenticate["login[password]"] = usernamePassword.Password;
            QueryParameters.LoginAuthenticate["login[_token]"] = loginToken;

            string loginData = uriController.ConcatenateQueryParameters(QueryParameters.LoginAuthenticate);

            var loginCheckResult = await stringNetworkController.PostString(Uris.Paths.Authentication.LoginCheck, null, loginData);

            if (!loginCheckResult.Contains("gogData"))
            {
                if (!loginCheckResult.Contains("second_step_authentication_token_letter"))
                    throw new System.Security.SecurityException(failedToAuthenticate);

                // 2FA is enabled for this user - ask for the code
                var securityCode = string.Empty;

                while (securityCode.Length != 4)
                {
                    consoleController.WriteLine(securityCodeHasBeenSent, MessageType.Default, usernamePassword.Username);
                    securityCode = consoleController.ReadLine();
                }

                var twoStepToken = extractionController.ExtractMultiple(loginCheckResult).First();

                QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_1]"] = securityCode[0].ToString();
                QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_2]"] = securityCode[1].ToString();
                QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_3]"] = securityCode[2].ToString();
                QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_4]"] = securityCode[3].ToString();
                QueryParameters.TwoStepAuthenticate["second_step_authentication[_token]"] = twoStepToken;

                string twoStepData = uriController.ConcatenateQueryParameters(QueryParameters.TwoStepAuthenticate);

                var twoStepLoginCheckResult = await stringNetworkController.PostString(Uris.Paths.Authentication.TwoStep, null, twoStepData);

                if (twoStepLoginCheckResult.Contains("gogData"))
                {
                    // successful login using 2FA or exception
                    consoleController.WriteLine(successfullyAuthorizedOnGOG, MessageType.Success, usernamePassword.Username);
                    return;
                }
                else throw new System.Security.SecurityException(failedToAuthenticate);
            }

            // successful login, and 2FA is not used for this user
            consoleController.WriteLine(successfullyAuthorizedOnGOG, MessageType.Success, usernamePassword.Username);
        }

        public async Task Deauthorize()
        {
            await stringNetworkController.GetString(Uris.Paths.Authentication.Logout);
        }
    }
}
