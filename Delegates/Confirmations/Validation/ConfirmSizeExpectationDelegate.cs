using System.IO;
using Interfaces.Delegates.Confirmations;

namespace Delegates.Confirmations.Validation
{
    public class ConfirmSizeExpectationDelegate: IConfirmExpectationDelegate<string, long>
    {
        public bool Confirm(string uri, long expectedSize)
        {
            return new FileInfo(uri).Length == expectedSize;
        }
    }
}