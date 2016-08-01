using System.Threading.Tasks;
using System.Linq;

using GOG.Interfaces;
using GOG.Interfaces.Settings;
using GOG.Models.Settings;
using GOG.Models.Uris;
using GOG.Models.QueryParameters;
using System;

namespace GOG.Controllers
{
    public class AuthorizationController : IAuthorizationController
    {

        // messages
        private const string authorizingOnGOG = "Authorizing {0} on GOG.com...";
        private const string successfullyAuthorizedOnGOG = "Successfully authorized {0} on GOG.com.";
        private const string failedToAuthenticate = "Failed to authenticate user with provided username and password.";
        private const string securityCodeHasBeenSent = "Enter security code that has been sent to {0}:";

        private IUriController uriController;
        private IStringNetworkController stringNetworkController;
        private ITokenExtractorController tokenExtractorController;
        private IConsoleController consoleController;

        public AuthorizationController(
            IUriController uriController,
            IStringNetworkController stringNetworkController,
            ITokenExtractorController tokenExtractorController,
            IConsoleController consoleController)
        {
            this.uriController = uriController;
            this.stringNetworkController = stringNetworkController;
            this.tokenExtractorController = tokenExtractorController;
            this.consoleController = consoleController;
        }

        public async Task<bool> Authorize(IAuthenticateProperties usernamePassword)
        {
            consoleController.WriteLine(authorizingOnGOG, ConsoleColor.Gray, usernamePassword.Username);

            // request authorization token

            string authResponse = await stringNetworkController.GetString(Uris.Paths.Authentication.Auth, QueryParameters.Authenticate);

            //string authResponse = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><title>Login ● GOG.com</title><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><link rel=\"stylesheet\" href=\"//static-login.gog.com/css/a6336e8-d3d3edf.css\"/></head><body class=\"_modal \"  data-content-type=\"loginForm\"><div data-content-type=\"loginForm\" class=\"_modal__box js-modal-box\"><button class=\"_modal__control js-close-modal\" data-action=\"close\"><i class=\"icn icn--close\"></i></button><form name=\"login\" method=\"post\" action=\"/login_check\" data-form-type=\"loginForm\" class=\"form form--login\"><h2 class=\"form__title\"><i class=\"icn icn--logo\"></i>Log in</h2><ol class=\"form__fieldset\"><li class=\"form__field field \"><label for=\"login_username\" class=\"required\">Email</label><input type=\"email\" id=\"login_username\" name=\"login[username]\" required=\"required\" type=\"email\" placeholder=\"Email\" class=\"field__input\" autocomplete=\"on\" required=\"required\" data-error=\"error_login_username\" data-input-type=\"user\" /><span id=\"error_login_username\" class=\"js-error-msg field__msg is-hidden\">Incorrect email</span></li><li class=\"form__field field \"><label for=\"login_password\" class=\"required\">Password</label><input type=\"password\" id=\"login_password\" name=\"login[password]\" required=\"required\" placeholder=\"Password\" class=\"field__input\" autocomplete=\"on\" required=\"required\" maxlength=\"4096\" data-error=\"error_login_password\" data-input-type=\"user\" /><span id=\"error_login_password\" class=\"js-error-msg field__msg is-hidden\">Password required</span></li><li class=\"form__field field btn-slot\"><button type=\"submit\" id=\"login_login\" name=\"login[login]\" class=\"btn btn--main\">Log in now</button></li></ol><div class=\"form__footer\"><div class=\"l-section\"><a class=\"btn js-normal-link\" data-content-type=\"requestPasswordForm\"           href =\"/password/request\">Reset password</a></div><div class=\"l-section\"><a class=\"btn js-change-content\" data-content-type=\"registerForm\">I’m new here</a></div></div><input type=\"hidden\" id=\"login__token\" name=\"login[_token]\" value=\"77WBHaoP84I0kMRmrqhsx52pmTXaR--_Cfk88uripz8\" /></form></div><div data-content-type=\"registerForm\" class=\"_modal__box js-modal-box\"><button class=\"_modal__control js-close-modal\" data-action=\"close\"><i class=\"icn icn--close\"></i></button><form name=\"register\" method=\"post\" action=\"/register\" data-form-type=\"registerForm\" class=\"form\"><h2 class=\"form__title\"><i class=\"icn icn--logo\"></i> Sign-up</h2><ol class=\"form__fieldset\"><li class=\"form__field field \"><label for=\"register_username\" class=\"required\">Username</label><input type=\"text\" id=\"register_username\" name=\"register[username]\" required=\"required\" placeholder=\"Username\" class=\"field__input\" autocomplete=\"on\" required=\"required\" data-error=\"error_register_username\" data-input-type=\"user\" /><span id=\"error_register_username\" class=\"js-error-msg field__msg is-hidden\">Username required</span><span data-error-type=\"exists\" class=\"js-ajax-error-msg field__msg is-hidden\">Username already taken</span><span data-error-type=\"illegal_chars\" class=\"js-ajax-error-msg field__msg is-hidden\">Forbidden special characters</span><span data-error-type=\"short\" class=\"js-ajax-error-msg field__msg is-hidden\">Username too short</span><span data-error-type=\"long\" class=\"js-ajax-error-msg field__msg is-hidden\">Username too long</span></li><li class=\"form__field field \"><label for=\"register_email\" class=\"required\">Email</label><input type=\"email\" id=\"register_email\" name=\"register[email]\" required=\"required\" class=\"field__input\" autocomplete=\"on\" required=\"required\" placeholder=\"Email\" data-error=\"error_register_email\" data-input-type=\"user\" /><span id=\"error_register_email\" class=\"js-error-msg field__msg is-hidden\">Incorrect email</span><span data-error-type=\"exists\" class=\"js-ajax-error-msg field__msg is-hidden\">Email address already used</span><span data-error-type=\"invalid\" class=\"js-ajax-error-msg field__msg is-hidden\">Incorrect email</span></li><li class=\"form__field field \"><label for=\"register_password\" class=\"required\">Password</label><input type=\"password\" id=\"register_password\" name=\"register[password]\" required=\"required\" placeholder=\"Password\" class=\"field__input\" autocomplete=\"on\" required=\"required\" data-error=\"error_register_password\" data-input-type=\"user\" /><span id=\"error_register_password\" class=\"js-error-msg field__msg is-hidden\">Password required</span></li><li class=\"form__field field btn-slot\"><button type=\"submit\" id=\"register_register\" name=\"register[register]\" class=\"btn btn--main\">Sign up now</button></li></ol><p class=\"form__description\">        By signing up you acknowledge that you are 13 or older and accept < a href =\"https://www.gog.com/support/policies\" target=\"_top\"><strong>GOG User Agreement and GOG Privacy Policy</strong></a>.      </ p >< div class=\"form__footer\"><div class=\"l-section\"><a class=\"btn js-change-content\" data-content-type=\"loginForm\">I already have an account</a></div></div><input type=\"hidden\" id=\"register__token\" name=\"register[_token]\" value=\"ZTl5XnHsL-v4zqYYnulE1ZaFHrOnwpxxAclYBqDrTgw\" /></form></div><script type=\"text/javascript\" src=\"//static-login.gog.com/js/2c420b1-19be331.js\"></script><script type=\"text/javascript\" src=\"//static-login.gog.com/js/69925ca-53367fb.js\"></script></body></html>";

            string loginToken = tokenExtractorController.ExtractMultiple(authResponse).First();

            // login using username / password

            QueryParameters.LoginAuthenticate["login[username]"] = usernamePassword.Username;
            QueryParameters.LoginAuthenticate["login[password]"] = usernamePassword.Password;
            QueryParameters.LoginAuthenticate["login[_token]"] = loginToken;

            string loginData = uriController.ConcatenateQueryParameters(QueryParameters.LoginAuthenticate);

            var loginCheckResult = await stringNetworkController.PostString(Uris.Paths.Authentication.LoginCheck, null, loginData);
            //var loginCheckResult = "<!DOCTYPE html><html lang=\"en\"><head>    <meta charset=\"utf-8\">    <title>Login ● GOG.com</title>    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">                            <link rel=\"stylesheet\" href=\"//static-login.gog.com/css/a6336e8-d3d3edf.css\"/>                    </head><body class=\"_modal \"  data-content-type=\"loginForm\"><div data-content-type=\"loginForm\" class=\"_modal__box js-modal-box\"><button class=\"_modal__control js-close-modal\" data-action=\"close\"><i class=\"icn icn--close\"></i></button><form name=\"login\" method=\"post\" action=\"/login_check\" data-form-type=\"loginForm\" class=\"form form--login\"><h2 class=\"form__title\"><i class=\"icn icn--logo\"></i>Log in</h2><ol class=\"form__fieldset\"><li class=\"form__field field \"><label for=\"login_username\" class=\"required\">Email</label><input type=\"email\" id=\"login_username\" name=\"login[username]\" required=\"required\" type=\"email\" placeholder=\"Email\" class=\"field__input\" autocomplete=\"on\" required=\"required\" data-error=\"error_login_username\" data-input-type=\"user\" /><span id=\"error_login_username\" class=\"js-error-msg field__msg is-hidden\">Incorrect email</span></li><li class=\"form__field field \"><label for=\"login_password\" class=\"required\">Password</label><input type=\"password\" id=\"login_password\" name=\"login[password]\" required=\"required\" placeholder=\"Password\" class=\"field__input\" autocomplete=\"on\" required=\"required\" maxlength=\"4096\" data-error=\"error_login_password\" data-input-type=\"user\" /><span id=\"error_login_password\" class=\"js-error-msg field__msg is-hidden\">Password required</span></li><li class=\"form__field field btn-slot\"><button type=\"submit\" id=\"login_login\" name=\"login[login]\" class=\"btn btn--main\">Log in now</button></li></ol><div class=\"form__footer\"><div class=\"l-section\"><a class=\"btn js-normal-link\" data-content-type=\"requestPasswordForm\"           href =\"/password/request\">Reset password</a></div><div class=\"l-section\"><a class=\"btn js-change-content\" data-content-type=\"registerForm\">I’m new here</a></div></div><input type=\"hidden\" id=\"login__token\" name=\"login[_token]\" value=\"2zY3vUGyYagsAUWGV0-GnayhQ8_88b-Q28DbsqTzOr4\" /></form></div><div data-content-type=\"registerForm\" class=\"_modal__box js-modal-box\"><button class=\"_modal__control js-close-modal\" data-action=\"close\"><i class=\"icn icn--close\"></i></button><form name=\"register\" method=\"post\" action=\"/register\" data-form-type=\"registerForm\" class=\"form\"><h2 class=\"form__title\"><i class=\"icn icn--logo\"></i> Sign-up</h2><ol class=\"form__fieldset\"><li class=\"form__field field \"><label for=\"register_username\" class=\"required\">Username</label><input type=\"text\" id=\"register_username\" name=\"register[username]\" required=\"required\" placeholder=\"Username\" class=\"field__input\" autocomplete=\"on\" required=\"required\" data-error=\"error_register_username\" data-input-type=\"user\" /><span id=\"error_register_username\" class=\"js-error-msg field__msg is-hidden\">Username required</span><span data-error-type=\"exists\" class=\"js-ajax-error-msg field__msg is-hidden\">Username already taken</span><span data-error-type=\"illegal_chars\" class=\"js-ajax-error-msg field__msg is-hidden\">Forbidden special characters</span><span data-error-type=\"short\" class=\"js-ajax-error-msg field__msg is-hidden\">Username too short</span><span data-error-type=\"long\" class=\"js-ajax-error-msg field__msg is-hidden\">Username too long</span></li><li class=\"form__field field \"><label for=\"register_email\" class=\"required\">Email</label><input type=\"email\" id=\"register_email\" name=\"register[email]\" required=\"required\" class=\"field__input\" autocomplete=\"on\" required=\"required\" placeholder=\"Email\" data-error=\"error_register_email\" data-input-type=\"user\" /><span id=\"error_register_email\" class=\"js-error-msg field__msg is-hidden\">Incorrect email</span><span data-error-type=\"exists\" class=\"js-ajax-error-msg field__msg is-hidden\">Email address already used</span><span data-error-type=\"invalid\" class=\"js-ajax-error-msg field__msg is-hidden\">Incorrect email</span></li><li class=\"form__field field \"><label for=\"register_password\" class=\"required\">Password</label><input type=\"password\" id=\"register_password\" name=\"register[password]\" required=\"required\" placeholder=\"Password\" class=\"field__input\" autocomplete=\"on\" required=\"required\" data-error=\"error_register_password\" data-input-type=\"user\" /><span id=\"error_register_password\" class=\"js-error-msg field__msg is-hidden\">Password required</span></li><li class=\"form__field field btn-slot\"><button type=\"submit\" id=\"register_register\" name=\"register[register]\" class=\"btn btn--main\">Sign up now</button></li></ol><p class=\"form__description\">        By signing up you acknowledge that you are 13 or older and accept < a href =\"https://www.gog.com/support/policies\" target=\"_top\"><strong>GOG User Agreement and GOG Privacy Policy</strong></a>.      </ p >< div class=\"form__footer\"><div class=\"l-section\"><a class=\"btn js-change-content\" data-content-type=\"loginForm\">I already have an account</a></div></div><input type=\"hidden\" id=\"register__token\" name=\"register[_token]\" value=\"_sCAwcefJvx6zVkzxVlTlTW_WfQVB8DnlwXHV0R5Npo\" /></form></div><script type=\"text/javascript\" src=\"//static-login.gog.com/js/2c420b1-19be331.js\"></script><script type=\"text/javascript\" src=\"//static-login.gog.com/js/69925ca-53367fb.js\"></script></body></html>";

            if (loginCheckResult.Contains("gogData"))
            {
                // successful login, and 2FA is not used for this user
                consoleController.WriteLine(successfullyAuthorizedOnGOG, ConsoleColor.Green, usernamePassword.Username);
                return true;
            }
            else
            {
                if (loginCheckResult.Contains("second_step_authentication_token_letter"))
                {
                    // 2FA is enabled for this user - ask for the code

                    var securityCode = string.Empty;

                    while (securityCode.Length != 4)
                    {
                        consoleController.WriteLine(securityCodeHasBeenSent, ConsoleColor.White, usernamePassword.Username);
                        securityCode = consoleController.ReadLine();
                    }

                    var twoStepToken = tokenExtractorController.ExtractMultiple(loginCheckResult).First();

                    QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_1]"] = securityCode[0].ToString();
                    QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_2]"] = securityCode[1].ToString();
                    QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_3]"] = securityCode[2].ToString();
                    QueryParameters.TwoStepAuthenticate["second_step_authentication[token][letter_4]"] = securityCode[3].ToString();
                    QueryParameters.TwoStepAuthenticate["second_step_authentication[_token]"] = twoStepToken;

                    string twoStepData = uriController.ConcatenateQueryParameters(QueryParameters.TwoStepAuthenticate);

                    var twoStepLoginCheckResult = await stringNetworkController.PostString(Uris.Paths.Authentication.TwoStep, null, twoStepData);

                    if (twoStepLoginCheckResult.Contains("gogData"))
                    {
                        // successful login using 2FA
                        consoleController.WriteLine(successfullyAuthorizedOnGOG, ConsoleColor.Green, usernamePassword.Username);
                        return true;
                    }
                    else {
                        consoleController.WriteLine(failedToAuthenticate, ConsoleColor.Red);
                        return false;
                    }
                }
                else
                {
                    consoleController.WriteLine(failedToAuthenticate, ConsoleColor.Red);
                    return false;
                }
            }
        }

        public async Task Deauthorize()
        {
            await stringNetworkController.GetString(Uris.Paths.Authentication.Logout);
        }
    }
}
