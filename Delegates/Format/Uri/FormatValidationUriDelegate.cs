using System.Linq;
using System.IO;

using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Format;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Delegates.Format.Uri
{
    public class FormatValidationUriDelegate : IFormatDelegate<string, string>
    {
        readonly IGetFilenameDelegate getValidationFilenameDelegate;
        readonly IFormatDelegate<string, string> formatUriRemoveSessionDelegate;

		[Dependencies(
            DependencyContext.Default,
			"Delegates.GetFilename.GetValidationFilenameDelegate,Delegates",
			"Delegates.Format.Uri.FormatUriRemoveSessionDelegate,Delegates")]
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