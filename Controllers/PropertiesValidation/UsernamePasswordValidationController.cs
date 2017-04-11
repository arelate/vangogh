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
            if (usernamePassword == null ||
                usernamePassword.Length < 2)
                usernamePassword = new string[2];

            var emptyUsername = string.IsNullOrEmpty(usernamePassword[0]);
            var emptyPassword = string.IsNullOrEmpty(usernamePassword[1]);

            // clear console before we request username / password
            if (emptyUsername || emptyPassword)
                consoleController.Clear();

            if (emptyUsername)
            {
                consoleController.WriteLine("Please enter your GOG.com username (email):");
                usernamePassword[0] = consoleController.ReadLine();
            }

            if (emptyPassword)
            {
                consoleController.WriteLine("Please enter password for {0}:", usernamePassword[0]);
                usernamePassword[1] = consoleController.InputPassword();
            }

            // and also clear console after we request username / password
            if (emptyUsername || emptyPassword)
                consoleController.Clear();

            return usernamePassword;
        }
    }
}
