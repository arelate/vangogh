using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GOG
{
    class AuthenticationController
    {
        public static async Task<bool> AuthorizeOnSite(ICredentials credentials, IConsoleController consoleController) {

            consoleController.WriteLine("Authorizing {0} on GOG.com...", credentials.Username);

            // request authorization token

            string authResponse = await NetworkController.RequestString(Urls.Authenticate, QueryParameters.Authenticate);

            // extracting login token that is 43 characters (letters, numbers...)
            Regex regex = new Regex(@"[\w-]{43}");
            var match = regex.Match(authResponse);
            string loginToken = match.Value;

            // login using username / password

            QueryParameters.LoginAuthenticate["login[username]"] = credentials.Username;
            QueryParameters.LoginAuthenticate["login[password]"] = credentials.Password;
            QueryParameters.LoginAuthenticate["login[_token]"] = loginToken;

            string loginData = NetworkController.CombineQueryParameters(QueryParameters.LoginAuthenticate);

            var loginCheckResult = await NetworkController.RequestString(Urls.LoginCheck, null, "POST", loginData);
            
            if (loginCheckResult.Contains("gogData"))
            {
                // successful login
                consoleController.WriteLine("Successfully authorized {0} on GOG.com.", credentials.Username);
                return true;
            } else
            {
                consoleController.WriteLine("Failed to authenticate user with provided username and password.");
                return false;
            }
        }
    }
}
