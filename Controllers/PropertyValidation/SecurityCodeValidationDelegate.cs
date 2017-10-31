using Interfaces.PropertyValidation;
using Interfaces.RequestData;

namespace Controllers.PropertyValidation
{
    public class SecurityCodeValidationDelegate : IValidatePropertiesDelegate<string>
    {
        private const string securityCodeHasBeenSent = "Enter four digits security code that has been sent to your email:";
        private IRequestDataController<string> requestDataController;

        public SecurityCodeValidationDelegate(IRequestDataController<string> requestDataController)
        {
            this.requestDataController = requestDataController;
        }

        public string ValidateProperties(string properties)
        {
            var securityCode = string.Empty;

            while (securityCode.Length != 4)
                securityCode = requestDataController.RequestData(securityCodeHasBeenSent);

            return securityCode;
        }
    }
}
