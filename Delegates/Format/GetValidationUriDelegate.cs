using System.Linq;
using System.IO;

using Interfaces.Delegates.GetFilename;

using Interfaces.Extraction;

namespace Delegates.Format
{
    public class FormatValidationUriDelegate : FormatUriDelegate
    {
        private IGetFilenameDelegate getValidationFilenameDelegate;
        private IStringExtractionController uriSansSessionExtractionController;

        public FormatValidationUriDelegate(
            IGetFilenameDelegate getValidationFilenameDelegate,
            IStringExtractionController uriSansSessionExtractionController)
        {
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;
            this.uriSansSessionExtractionController = uriSansSessionExtractionController;
        }

        public override string Format(string sourceUri)
        {
            var sourceUriSansSession = uriSansSessionExtractionController.ExtractMultiple(sourceUri).Single();
            var sourceFilename = Path.GetFileName(sourceUriSansSession);
            var validationFilename = getValidationFilenameDelegate.GetFilename(sourceFilename);

            return sourceUri.Replace(sourceFilename, validationFilename);
        }
    }
}