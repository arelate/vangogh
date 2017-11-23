using Interfaces.PropertyValidation;
using Interfaces.Input;

namespace Controllers.PropertyValidation
{
    public class SecurityCodeValidationDelegate : IValidatePropertiesDelegate<string>
    {
        private const string securityCodeHasBeenSent = "Enter four digits security code that has been sent to your email:";
        private IRequestInputDelegate<string> requestInputDelegate;

        public SecurityCodeValidationDelegate(IRequestInputDelegate<string> requestInputDelegate)
        {
            this.requestInputDelegate = requestInputDelegate;
        }

        public string ValidateProperties(string properties)
        {
            var securityCode = string.Empty;

            while (securityCode.Length != 4)
                securityCode = requestInputDelegate.RequestInput(securityCodeHasBeenSent);

            return securityCode;
        }
    }
}
