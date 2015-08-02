using System.Threading.Tasks;
using System.Text.RegularExpressions;

using GOG.Interfaces;
using GOG.SharedModels;

namespace GOG.Controllers
{
    public class AuthenticationController: IAuthenticationController
    {
        private IUriController uriController;
        private IStringNetworkController stringNetworkController;
        private IConsoleController consoleController;

        public AuthenticationController(
            IUriController uriController, 
            IStringNetworkController stringNetworkController,
            IConsoleController consoleController)
        {
            this.uriController = uriController;
            this.stringNetworkController = stringNetworkController;
            this.consoleController = consoleController;
        }

        public async Task<bool> Authorize(ICredentials credentials)
        {
            consoleController.WriteLine("Authorizing {0} on GOG.com...", credentials.Username);

            // request authorization token

            string authResponse = await stringNetworkController.GetString(Urls.Authenticate, QueryParameters.Authenticate);

            // extracting login token that is 43 characters (letters, numbers, - ...)
            Regex regex = new Regex(@"[\w-]{43}");
            var match = regex.Match(authResponse);
            string loginToken = match.Value;

            // login using username / password

            //QueryParameters.LoginAuthenticate["login[username]"] = credentials.Username;
            QueryParameters.LoginAuthenticate["login[password]"] = credentials.Password;
            QueryParameters.LoginAuthenticate["login[_token]"] = loginToken;

            string loginData = uriController.CombineQueryParameters(QueryParameters.LoginAuthenticate);

            //var loginCheckResult = await stringNetworkController.PostString(Urls.LoginCheck, null, loginData);
            var loginCheckResult = await stringNetworkController.PostString(Urls.Login, null, loginData);


            if (loginCheckResult.Contains("gogData"))
            {
                // successful login
                consoleController.WriteLine("Successfully authorized {0} on GOG.com.", credentials.Username);
                return true;
            }
            else
            {
                consoleController.WriteLine("Failed to authenticate user with provided username and password.");
                return false;
            }
        }
    }
}
