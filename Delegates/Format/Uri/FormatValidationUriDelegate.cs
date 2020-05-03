using System.IO;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Format;
using Attributes;
using Delegates.Values.Filenames;

namespace Delegates.Format.Uri
{
    public class FormatValidationUriDelegate : IFormatDelegate<string, string>
    {
        private readonly IGetValueDelegate<string, string> getValidationFilenameDelegate;
        private readonly IFormatDelegate<string, string> formatUriRemoveSessionDelegate;

        [Dependencies(
            typeof(GetValidationFilenameDelegate),
            typeof(FormatUriRemoveSessionDelegate))]
        public FormatValidationUriDelegate(
            IGetValueDelegate<string, string> getValidationFilenameDelegate,
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate)
        {
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;
            this.formatUriRemoveSessionDelegate = formatUriRemoveSessionDelegate;
        }

        public string Format(string sourceUri)
        {
            var sourceUriSansSession = formatUriRemoveSessionDelegate.Format(sourceUri);
            var sourceFilename = Path.GetFileName(sourceUriSansSession);
            var validationFilename = getValidationFilenameDelegate.GetValue(sourceFilename);

            return sourceUri.Replace(sourceFilename, validationFilename);
        }
    }
}