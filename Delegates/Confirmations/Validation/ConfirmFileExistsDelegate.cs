using System.IO;
using Interfaces.Delegates.Confirmations;

namespace Delegates.Confirmations.Validation
{
    public class ConfirmFileExistsDelegate: IConfirmDelegate<string>
    {
        public bool Confirm(string validationFileUri)
        {
            return File.Exists(validationFileUri);
        }
    }
}