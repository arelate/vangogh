using System.Threading.Tasks;

using Interfaces.PropertyValidation;
using Interfaces.Input;

namespace Controllers.PropertyValidation
{
    public class UsernamePasswordValidationDelegate : IValidatePropertiesAsyncDelegate<string[]>
    {
        private IInputController<string> inputController;

        public UsernamePasswordValidationDelegate(IInputController<string> inputController)
        {
            this.inputController = inputController;
        }

        public async Task<string[]> ValidatePropertiesAsync(string[] usernamePassword)
        {
            if (usernamePassword == null ||
                usernamePassword.Length < 2)
                usernamePassword = new string[2];

            var emptyUsername = string.IsNullOrEmpty(usernamePassword[0]);
            var emptyPassword = string.IsNullOrEmpty(usernamePassword[1]);

            if (emptyUsername)
                usernamePassword[0] =
                    await inputController.RequestInputAsync("Please enter your GOG.com username (email):");

            if (emptyPassword)
                usernamePassword[1] =
                    await inputController.RequestPrivateInputAsync(
                        string.Format(
                            "Please enter password for {0}:", 
                            usernamePassword[0]));

            return usernamePassword;
        }
    }
}
