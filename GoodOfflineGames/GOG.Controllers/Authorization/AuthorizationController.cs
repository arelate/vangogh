using System.Threading.Tasks;
using System.Linq;

using GOG.Interfaces.Authorization;

using Interfaces.Uri;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Console;
using Interfaces.Serialization;
using Interfaces.PropertiesValidation;

using Models.Uris;
using Models.QueryParameters;

namespace GOG.Controllers.Authorization
{
    public class AuthorizationController : IAuthorizationController
    {
        private const string failedToAuthenticate = "Failed to authenticate user with provided username and password.";
        private const string recaptchaDetected = "Login page contains reCAPTCHA.\n" +
            "Please login in the browser, then export the galaxy-login-* cookies into ./cookies.json\n" +
            "{INSTRUCTIONS}";
        private const string securityCodeHasBeenSent = "Enter security code that has been sent to {0}:";

        private const string recaptchaUri = "https://www.google.com/recaptcha";

        private IValidatePropertiesDelegate<string> usernamePasswordValidationDelegate;
        private IUriController uriController;
        private INetworkController networkController;
        private IStringExtractionController loginTokenExtractionController;
        private IStringExtractionController loginIdExtractionController;
        private IStringExtractionController loginUsernameExtractionController;
        private IConsoleController consoleController;
        private ISerializationController<string> serializationController;

        public AuthorizationController(
            IValidatePropertiesDelegate<string> usernamePasswordValidationDelegate,
            IUriController uriController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IStringExtractionController loginTokenExtractionController,
            IStringExtractionController loginIdExtractionController,
            IStringExtractionController loginUsernameExtractionController,
            IConsoleController consoleController)
        {
            this.usernamePasswordValidationDelegate = usernamePasswordValidationDelegate;
            this.uriController = uriController;
            this.networkController = networkController;
            this.loginTokenExtractionController = loginTokenExtractionController;
            this.loginIdExtractionController = loginIdExtractionController;
            this.loginUsernameExtractionController = loginUsernameExtractionController;
            this.consoleController = consoleController;
            this.serializationController = serializationController;
        }

        public async Task<bool> IsAuthorized()
        {
            var userDataString = await networkController.Get(Uris.Paths.Authentication.UserData);
            if (string.IsNullOrEmpty(userDataString)) return false;

            var userData = serializationController.Deserialize<Models.UserData>(userDataString);

            return userData.IsLoggedIn;
        }

        public async Task Authorize(string username, string password)
        {
            if (await IsAuthorized()) return;

            // request authorization token
            string authResponse = await networkController.Get(Uris.Paths.Authentication.Auth, QueryParameters.Authenticate);

            if (authResponse.Contains(recaptchaUri))
            {
                throw new System.Security.SecurityException(recaptchaDetected);
            }

            string loginToken = loginTokenExtractionController.ExtractMultiple(authResponse).First();

            // login using username / password or login id / password
            var loginUri = string.Empty;
            if (authResponse.Contains("login[id]"))
            {
                var loginId = loginIdExtractionController.ExtractMultiple(authResponse).First();
                QueryParameters.LoginAuthenticate.Remove("login[username]");
                QueryParameters.LoginAuthenticate["login[id]"] = loginId;
                loginUri = Uris.Paths.Authentication.Login;

                username = loginUsernameExtractionController.ExtractMultiple(authResponse).First();
            }
            else
            {
                QueryParameters.LoginAuthenticate.Remove("login[id]");
                loginUri = Uris.Paths.Authentication.LoginCheck;
            }

            var usernamePassword = usernamePasswordValidationDelegate.ValidateProperties(username, password);

            QueryParameters.LoginAuthenticate["login[username]"] = usernamePassword[0];
            QueryParameters.LoginAuthenticate["login[password]"] = usernamePassword[1];
            QueryParameters.LoginAuthenticate["login[_token]"] = loginToken;

            string loginData = uriController.ConcatenateQueryParameters(QueryParameters.LoginAuthenticate);

            var loginCheckResult = await networkController.Post(loginUri, null, loginData);

            // login attempt was successful
            if (loginCheckResult.Contains("gogData"))
                return;

            if (!loginCheckResult.Contains("second_step_authentication_token_letter"))
                throw new System.Security.SecurityException(failedToAuthenticate);

            // 2FA is enabled for this user - ask for the code
            var securityCode = string.Empty;

            while (securityCode.Length != 4)
            {
                consoleController.WriteLine(securityCodeHasBeenSent, null, usernamePassword[0]);
                securityCode = consoleController.ReadLine();
            }

            var twoStepToken = loginTokenExtractionController.ExtractMultiple(loginCheckResult).First();

            QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_1]"] = securityCode[0].ToString();
            QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_2]"] = securityCode[1].ToString();
            QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_3]"] = securityCode[2].ToString();
            QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_4]"] = securityCode[3].ToString();
            QueryParameters.TwoStepAuthenticate["second_step_authentication[_token]"] = twoStepToken;

            string twoStepData = uriController.ConcatenateQueryParameters(QueryParameters.TwoStepAuthenticate);

            var twoStepLoginCheckResult = await networkController.Post(Uris.Paths.Authentication.TwoStep, null, twoStepData);

            if (twoStepLoginCheckResult.Contains("gogData"))
                return;

            throw new System.Security.SecurityException(failedToAuthenticate);
        }

        public async Task Deauthorize()
        {
            await networkController.Get(Uris.Paths.Authentication.Logout);
        }
    }
}
