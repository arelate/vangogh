using System.Linq;
using System.IO;

using Interfaces.Delegates.GetUri;
using Interfaces.Delegates.GetFilename;

using Interfaces.Extraction;

namespace Delegates.GetUri
{
    public class GetValidationUriDelegate : IGetUriDelegate
    {
        private IGetFilenameDelegate getValidationFilenameDelegate;
        private IStringExtractionController uriSansSessionExtractionController;

        public GetValidationUriDelegate(
            IGetFilenameDelegate getValidationFilenameDelegate,
            IStringExtractionController uriSansSessionExtractionController)
        {
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;
            this.uriSansSessionExtractionController = uriSansSessionExtractionController;
        }

        public string GetUri(string sourceUri)
        {
            var sourceUriSansSession = uriSansSessionExtractionController.ExtractMultiple(sourceUri).Single();
            var sourceFilename = Path.GetFileName(sourceUriSansSession);
            var validationFilename = getValidationFilenameDelegate.GetFilename(sourceFilename);

            return sourceUri.Replace(sourceFilename, validationFilename);
        }
    }
}