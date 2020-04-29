using System.IO;
using Interfaces.Delegates.Confirm;

namespace Delegates.Confirm.Validation
{
    public class ConfirmFileExistsDelegate: IConfirmDelegate<string>
    {
        public bool Confirm(string validationFileUri)
        {
            return File.Exists(validationFileUri);
        }
    }
}