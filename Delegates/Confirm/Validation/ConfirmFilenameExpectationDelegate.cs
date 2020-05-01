using System.IO;
using Interfaces.Delegates.Confirm;

namespace Delegates.Confirm.Validation
{
    public class ConfirmFilenameExpectationDelegate: IConfirmExpectationDelegate<string, string>
    {
        public bool Confirm(string uri, string expectedFilename)
        {
            return Path.GetFileName(uri) == expectedFilename;
        }
    }
}