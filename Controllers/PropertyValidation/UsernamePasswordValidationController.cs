using Interfaces.PropertyValidation;
using Interfaces.Settings;
using Interfaces.Console;

namespace Controllers.PropertyValidation
{
    public class UsernamePasswordValidationDelegate : IValidatePropertiesDelegate<string[]>
    {
        private IConsoleController consoleController;

        public UsernamePasswordValidationDelegate(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public string[] ValidateProperties(string[] usernamePassword)
        {
            if (usernamePassword == null ||
                usernamePassword.Length < 2)
                usernamePassword = new string[2];

            var emptyUsername = string.IsNullOrEmpty(usernamePassword[0]);
            var emptyPassword = string.IsNullOrEmpty(usernamePassword[1]);

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

            return usernamePassword;
        }
    }
}
