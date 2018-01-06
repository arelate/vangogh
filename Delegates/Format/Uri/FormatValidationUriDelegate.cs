using System.Linq;
using System.IO;

using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Format;

namespace Delegates.Format.Uri
{
    public class FormatValidationUriDelegate : IFormatDelegate<string, string>
    {
        private IGetFilenameDelegate getValidationFilenameDelegate;
        private IFormatDelegate<string, string> formatUriRemoveSessionDelegate;

        public FormatValidationUriDelegate(
            IGetFilenameDelegate getValidationFilenameDelegate,
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate)
        {
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;
            this.formatUriRemoveSessionDelegate = formatUriRemoveSessionDelegate;
        }

        public string Format(string sourceUri)
        {
            var sourceUriSansSession = formatUriRemoveSessionDelegate.Format(sourceUri);
            var sourceFilename = Path.GetFileName(sourceUriSansSession);
            var validationFilename = getValidationFilenameDelegate.GetFilename(sourceFilename);

            return sourceUri.Replace(sourceFilename, validationFilename);
        }
    }
}