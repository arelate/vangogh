using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    class Auth
    {
        // TODO: Authenticate on server to get available games
        public static async Task AuthorizeOnSite(ICredentials credentials, IConsoleController consoleController) {

            consoleController.WriteLine("Authorizing {0} on GOG.com...", credentials.Username);

            // request authorization token

            string authResponse = await Network.Request(Urls.Authenticate, QueryParameters.Authenticate);

            List<Dictionary<string, string>> loginTokenAttributesValues = HTMLHelper.ExtractAttributesValues(
                authResponse,
                new List<string>() { "login__token" },
                new List<string>() { "value" });

            string loginToken = string.Empty;
            if (loginTokenAttributesValues.Count > 0)
            {
                loginToken = loginTokenAttributesValues[0]["value"];
            }

            // login using username / password

            QueryParameters.LoginAuthenticate["login[username]"] = credentials.Username;
            QueryParameters.LoginAuthenticate["login[password]"] = credentials.Password;
            QueryParameters.LoginAuthenticate["login[_token]"] = loginToken;

            string loginData = Network.CombineQueryParameters(QueryParameters.LoginAuthenticate);

            await Network.Request(Urls.LoginCheck, null, "POST", loginData);

            consoleController.WriteLine("Successfully authorized {0} on GOG.com.", credentials.Username);
        }
    }
}
