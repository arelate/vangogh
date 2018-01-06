using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Input;

namespace Delegates.Correct
{
    public class CorrectSecurityCodeAsyncDelegate : ICorrectAsyncDelegate<string>
    {
        private const string securityCodeHasBeenSent = 
            "Enter four digits security code that has been sent to your email:";
        private IRequestInputAsyncDelegate<string> requestInputDelegate;

        public CorrectSecurityCodeAsyncDelegate(IRequestInputAsyncDelegate<string> requestInputDelegate)
        {
            this.requestInputDelegate = requestInputDelegate;
        }

        public async Task<string> CorrectAsync(string properties)
        {
            var securityCode = string.Empty;

            while (securityCode.Length != 4)
                securityCode = await requestInputDelegate.RequestInputAsync(securityCodeHasBeenSent);

            return securityCode;
        }
    }
}
