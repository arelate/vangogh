using System.Collections.Generic;
using System.IO;

using Interfaces.Delegates.Format;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.Format.Uri
{
    public class FormatValidationFileDelegate : IFormatDelegate<string, string>
    {
        private IGetDirectoryDelegate validationDirectoryDelegate;
        private IGetFilenameDelegate validationFilenameDelegate;

        public FormatValidationFileDelegate(
            IGetDirectoryDelegate validationDirectoryDelegate,
            IGetFilenameDelegate validationFilenameDelegate)
        {
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.validationFilenameDelegate = validationFilenameDelegate;
        }

        public string Format(string uri)
        {
            return Path.Combine(
                    validationDirectoryDelegate.GetDirectory(string.Empty),
                    validationFilenameDelegate.GetFilename(
                        Path.GetFileName(uri)));
        }
    }
}
