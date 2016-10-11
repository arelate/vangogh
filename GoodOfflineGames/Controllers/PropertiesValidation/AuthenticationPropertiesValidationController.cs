using Interfaces.PropertiesValidation;
using Interfaces.Settings;
using Interfaces.Console;

namespace Controllers.PropertiesValidation
{
    public class AuthenticationPropertiesValidationController : IAuthenticationPropertiesValidationController
    {
        private IConsoleController consoleController;

        public AuthenticationPropertiesValidationController(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public IAuthenticationProperties ValidateProperties(IAuthenticationProperties authenticationProperties)
        {
            if (authenticationProperties == null)
                authenticationProperties = new Models.Settings.AuthenticationProperties();

            if (string.IsNullOrEmpty(authenticationProperties?.Username))
            {
                consoleController.WriteLine("Please enter your GOG.com username (email):");
                authenticationProperties.Username = consoleController.ReadLine();
            }

            if (string.IsNullOrEmpty(authenticationProperties?.Password))
            {
                consoleController.WriteLine("Please enter password for {0}:", MessageType.Default, authenticationProperties.Username);
                authenticationProperties.Password = consoleController.InputPassword();
            }

            return authenticationProperties;
        }
    }
}
