using System.Collections.Generic;
using System.IO;

using Interfaces.Delegates.Format;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.Format.Uri
{
    public class FormatValidationFileDelegate : IFormatDelegate<string, string>
    {
        private IGetDirectoryAsyncDelegate validationDirectoryDelegate;
        private IGetFilenameDelegate validationFilenameDelegate;

        public FormatValidationFileDelegate(
            IGetDirectoryAsyncDelegate validationDirectoryDelegate,
            IGetFilenameDelegate validationFilenameDelegate)
        {
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.validationFilenameDelegate = validationFilenameDelegate;
        }

        public string Format(string uri)
        {
            return Path.Combine(
                    // TODO: Clearly fix this
                    validationDirectoryDelegate.GetDirectoryAsync(string.Empty, null).Result,
                    validationFilenameDelegate.GetFilename(
                        Path.GetFileName(uri)));
        }
    }
}
