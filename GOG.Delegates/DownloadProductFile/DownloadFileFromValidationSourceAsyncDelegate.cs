using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetUri;

using Interfaces.Controllers.File;

using Interfaces.Extraction;
using Interfaces.Expectation;
using Interfaces.Enumeration;
using Interfaces.FileDownload;
using Interfaces.Status;

using GOG.Interfaces.Delegates.DownloadProductFile;

namespace GOG.Delegates.DownloadFileFromSource
{
    public class DownloadValidationFileAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        private IStringExtractionController uriSansSessionExtractionController;
        private IExpectedDelegate<string> validationExpectedForUriDelegate;
        private IEnumerateDelegate<string> validationFileEnumerateDelegate;
        private IGetDirectoryDelegate validationDirectoryDelegate;
        private IGetUriDelegate validationUriDelegate;
        private IFileController fileController;
        private IDownloadFileFromSourceAsyncDelegate downloadFileFromSourceAsyncDelegate;
        private IStatusController statusController;

        public DownloadValidationFileAsyncDelegate(
            IStringExtractionController uriSansSessionExtractionController,
            IExpectedDelegate<string> validationExpectedForUriDelegate,
            IEnumerateDelegate<string> validationFileEnumerateDelegate,
            IGetDirectoryDelegate validationDirectoryDelegate,
            IGetUriDelegate validationUriDelegate,
            IFileController fileController,
            IDownloadFileFromSourceAsyncDelegate downloadFileFromSourceAsyncDelegate,
            IStatusController statusController)
        {
            this.uriSansSessionExtractionController = uriSansSessionExtractionController;
            this.validationExpectedForUriDelegate = validationExpectedForUriDelegate;
            this.validationFileEnumerateDelegate = validationFileEnumerateDelegate;
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.validationUriDelegate = validationUriDelegate;
            this.fileController = fileController;
            this.downloadFileFromSourceAsyncDelegate = downloadFileFromSourceAsyncDelegate;
            this.statusController = statusController;
        }

        public async Task DownloadProductFileAsync(long id, string title, string sourceUri, string destination, IStatus status)
        {
            if (string.IsNullOrEmpty(sourceUri)) return;

            var sourceUriSansSession = uriSansSessionExtractionController.ExtractMultiple(sourceUri).Single();
            var destinationUri = validationFileEnumerateDelegate.Enumerate(sourceUriSansSession).Single();
            
            // return early if validation is not expected for this file
            if (!validationExpectedForUriDelegate.Expected(sourceUriSansSession)) return;

            if (fileController.Exists(destinationUri))
            {
                await statusController.InformAsync(status, "Validation file already exists, will not be redownloading");
                return;
            }

            var validationSourceUri = validationUriDelegate.GetUri(sourceUri);

            var downloadValidationFileTask = await statusController.CreateAsync(status, "Download validation file");

            await downloadFileFromSourceAsyncDelegate.DownloadFileFromSourceAsync(
                validationSourceUri,
                validationDirectoryDelegate.GetDirectory(),
                downloadValidationFileTask);

            await statusController.CompleteAsync(downloadValidationFileTask);
        }
    }
}
