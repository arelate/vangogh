using System.IO;
using Interfaces.Delegates.Confirm;

namespace Delegates.Confirm.Validation
{
    public class ConfirmSizeExpectationDelegate: IConfirmExpectationDelegate<string, long>
    {
        public bool Confirm(string uri, long expectedSize)
        {
            return new FileInfo(uri).Length == expectedSize;
        }
    }
}