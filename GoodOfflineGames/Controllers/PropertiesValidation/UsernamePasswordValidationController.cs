using Interfaces.PropertiesValidation;
using Interfaces.Settings;
using Interfaces.Console;

namespace Controllers.PropertiesValidation
{
    public class UsernamePasswordValidationDelegate : IValidatePropertiesDelegate<string>
    {
        private IConsoleController consoleController;

        public UsernamePasswordValidationDelegate(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public string[] ValidateProperties(params string[] usernamePassword)
        {
            var validatedUsernamePassword = new string[2];

            if (usernamePassword == null ||
                usernamePassword.Length < 2)
                usernamePassword = new string[2];

            if (string.IsNullOrEmpty(usernamePassword[0]))
            {
                consoleController.WriteLine("Please enter your GOG.com username (email):");
                validatedUsernamePassword[0] = consoleController.ReadLine();
            }

            if (string.IsNullOrEmpty(usernamePassword[1]))
            {
                consoleController.WriteLine("Please enter password for {0}:", null, validatedUsernamePassword[0]);
                validatedUsernamePassword[1] = consoleController.InputPassword();
            }

            return validatedUsernamePassword;
        }
    }
}
