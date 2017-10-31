using Interfaces.PropertyValidation;
using Interfaces.RequestData;

namespace Controllers.PropertyValidation
{
    public class UsernamePasswordValidationDelegate : IValidatePropertiesDelegate<string[]>
    {
        private IRequestDataController<string> requestDataController;

        public UsernamePasswordValidationDelegate(IRequestDataController<string> requestDataController)
        {
            this.requestDataController = requestDataController;
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
                    requestDataController.RequestData("Please enter your GOG.com username (email):");

            if (emptyPassword)
                usernamePassword[1] =
                    requestDataController.RequestPrivateData(
                        string.Format(
                            "Please enter password for {0}:", 
                            usernamePassword[0]));

            return usernamePassword;
        }
    }
}
