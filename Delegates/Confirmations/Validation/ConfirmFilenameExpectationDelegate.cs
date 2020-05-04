using System.IO;
using Interfaces.Delegates.Confirmations;

namespace Delegates.Confirmations.Validation
{
    public class ConfirmFilenameExpectationDelegate: IConfirmExpectationDelegate<string, string>
    {
        public bool Confirm(string uri, string expectedFilename)
        {
            return Path.GetFileName(uri) == expectedFilename;
        }
    }
}