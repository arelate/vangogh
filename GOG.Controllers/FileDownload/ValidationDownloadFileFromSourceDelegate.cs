using System.Linq;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Expectation;
using Interfaces.Enumeration;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Uri;
using Interfaces.File;
using Interfaces.FileDownload;
using Interfaces.Status;

namespace GOG.Controllers.FileDownload
{
    public class ValidationDownloadFileFromSourceDelegate : IDownloadFileFromSourceAsyncDelegate
    {
        private IStringExtractionController uriSansSessionExtractionController;
        private IExpectedDelegate<string> validationExpectedForUriDelegate;
        private IEnumerateDelegate<string> validationFileEnumerateDelegate;
        private IGetDirectoryDelegate validationDirectoryDelegate;
        private IGetUriDelegate validationUriDelegate;
        private IFileController fileController;
        private IFileDownloadController fileDownloadController;
        private IStatusController statusController;

        public ValidationDownloadFileFromSourceDelegate(
            IStringExtractionController uriSansSessionExtractionController,
            IExpectedDelegate<string> validationExpectedForUriDelegate,
            IEnumerateDelegate<string> validationFileEnumerateDelegate,
            IGetDirectoryDelegate validationDirectoryDelegate,
            IGetUriDelegate validationUriDelegate,
            IFileController fileController,
            IFileDownloadController fileDownloadController,
            IStatusController statusController)
        {
            this.uriSansSessionExtractionController = uriSansSessionExtractionController;
            this.validationExpectedForUriDelegate = validationExpectedForUriDelegate;
            this.validationFileEnumerateDelegate = validationFileEnumerateDelegate;
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.validationUriDelegate = validationUriDelegate;
            this.fileController = fileController;
            this.fileDownloadController = fileDownloadController;
            this.statusController = statusController;
        }

        public async Task DownloadFileFromSourceAsync(long id, string title, string sourceUri, string destination, IStatus status)
        {
            if (string.IsNullOrEmpty(sourceUri)) return;

            var sourceUriSansSession = uriSansSessionExtractionController.ExtractMultiple(sourceUri).Single();
            var destinationUri = validationFileEnumerateDelegate.Enumerate(sourceUriSansSession).Single();
            
            // return early if validation is not expected for this file
            if (!validationExpectedForUriDelegate.Expected(sourceUriSansSession)) return;

            if (fileController.Exists(destinationUri))
            {
                statusController.Inform(status, "Validation file already exists, will not be redownloading");
                return;
            }

            var validationSourceUri = validationUriDelegate.GetUri(sourceUri);

            var downloadValidationFileTask = statusController.Create(status, "Download validation file");

            await fileDownloadController.DownloadFileFromSourceAsync(
                id,
                title,
                validationSourceUri,
                validationDirectoryDelegate.GetDirectory(),
                downloadValidationFileTask);

            statusController.Complete(downloadValidationFileTask);
        }
    }
}
