using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Input;

using Attributes;

namespace Delegates.Correct
{
    public class CorrectSecurityCodeAsyncDelegate : ICorrectAsyncDelegate<string>
    {
        const string securityCodeHasBeenSent = 
            "Enter four digits security code that has been sent to your email:";
        readonly IRequestInputAsyncDelegate<string> requestInputDelegate;

        [Dependencies("Controllers.InputOutput.ConsoleInputOutputController,Controllers")]
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
