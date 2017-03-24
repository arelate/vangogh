using System.Linq;
using System.IO;

using Interfaces.Destination.Uri;
using Interfaces.Destination.Filename;
using Interfaces.Extraction;

namespace Controllers.Destination.Uri
{
    public class ValidationUriDelegate : IGetUriDelegate
    {
        private IGetFilenameDelegate getValidationFilenameDelegate;
        private IStringExtractionController uriSansSessionExtractionController;

        public ValidationUriDelegate(
            IGetFilenameDelegate getValidationFilenameDelegate,
            IStringExtractionController uriSansSessionExtractionController)
        {
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;
            this.uriSansSessionExtractionController = uriSansSessionExtractionController;
        }

        public string GetUri(string sourceUri)
        {
            var sourceUriSansSession = uriSansSessionExtractionController.ExtractMultiple(sourceUri).First();
            var sourceFilename = Path.GetFileName(sourceUriSansSession);
            var validationFilename = getValidationFilenameDelegate.GetFilename(sourceFilename);

            return sourceUri.Replace(sourceFilename, validationFilename);
        }
    }
}