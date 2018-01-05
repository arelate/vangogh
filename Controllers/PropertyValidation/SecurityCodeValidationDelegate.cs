using System.Threading.Tasks;

using Interfaces.PropertyValidation;
using Interfaces.Input;

namespace Controllers.PropertyValidation
{
    public class SecurityCodeValidationDelegate : IValidatePropertiesAsyncDelegate<string>
    {
        private const string securityCodeHasBeenSent = "Enter four digits security code that has been sent to your email:";
        private IRequestInputAsyncDelegate<string> requestInputDelegate;

        public SecurityCodeValidationDelegate(IRequestInputAsyncDelegate<string> requestInputDelegate)
        {
            this.requestInputDelegate = requestInputDelegate;
        }

        public async Task<string> ValidatePropertiesAsync(string properties)
        {
            var securityCode = string.Empty;

            while (securityCode.Length != 4)
                securityCode = await requestInputDelegate.RequestInputAsync(securityCodeHasBeenSent);

            return securityCode;
        }
    }
}
