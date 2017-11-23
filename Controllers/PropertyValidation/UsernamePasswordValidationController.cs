using Interfaces.PropertyValidation;
using Interfaces.Input;

namespace Controllers.PropertyValidation
{
    public class UsernamePasswordValidationDelegate : IValidatePropertiesDelegate<string[]>
    {
        private IInputController<string> inputController;

        public UsernamePasswordValidationDelegate(IInputController<string> inputController)
        {
            this.inputController = inputController;
        }

        public string[] ValidateProperties(string[] usernamePassword)
        {
            if (usernamePassword == null ||
                usernamePassword.Length < 2)
                usernamePassword = new string[2];

            var emptyUsername = string.IsNullOrEmpty(usernamePassword[0]);
            var emptyPassword = string.IsNullOrEmpty(usernamePassword[1]);

            if (emptyUsername)
                usernamePassword[0] =
                    inputController.RequestInput("Please enter your GOG.com username (email):");

            if (emptyPassword)
                usernamePassword[1] =
                    inputController.RequestPrivateInput(
                        string.Format(
                            "Please enter password for {0}:", 
                            usernamePassword[0]));

            return usernamePassword;
        }
    }
}
