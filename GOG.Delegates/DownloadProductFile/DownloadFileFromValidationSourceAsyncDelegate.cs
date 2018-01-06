using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Confirm;

using Interfaces.Controllers.File;

using Interfaces.FileDownload;
using Interfaces.Status;

using GOG.Interfaces.Delegates.DownloadProductFile;

namespace GOG.Delegates.DownloadFileFromSource
{
    public class DownloadValidationFileAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        private IFormatDelegate<string, string> formatUriRemoveSessionDelegate;
        private IConfirmDelegate<string> confirmValidationExpectedDelegate;
        private IFormatDelegate<string, string> formatValidationFileDelegate;
        private IGetDirectoryDelegate validationDirectoryDelegate;
        private IFormatDelegate<string, string> formatValidationUriDelegate;
        private IFileController fileController;
        private IDownloadFileFromSourceAsyncDelegate downloadFileFromSourceAsyncDelegate;
        private IStatusController statusController;

        public DownloadValidationFileAsyncDelegate(
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate,
            IConfirmDelegate<string> confirmValidationExpectedDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IGetDirectoryDelegate validationDirectoryDelegate,
            IFormatDelegate<string, string> formatValidationUriDelegate,
            IFileController fileController,
            IDownloadFileFromSourceAsyncDelegate downloadFileFromSourceAsyncDelegate,
            IStatusController statusController)
        {
            this.formatUriRemoveSessionDelegate = formatUriRemoveSessionDelegate;
            this.confirmValidationExpectedDelegate = confirmValidationExpectedDelegate;
            this.formatValidationFileDelegate = formatValidationFileDelegate;
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.formatValidationUriDelegate = formatValidationUriDelegate;
            this.fileController = fileController;
            this.downloadFileFromSourceAsyncDelegate = downloadFileFromSourceAsyncDelegate;
            this.statusController = statusController;
        }

        public async Task DownloadProductFileAsync(long id, string title, string sourceUri, string destination, IStatus status)
        {
            if (string.IsNullOrEmpty(sourceUri)) return;

            var sourceUriSansSession = formatUriRemoveSessionDelegate.Format(sourceUri);
            var destinationUri = formatValidationFileDelegate.Format(sourceUriSansSession);
            
            // return early if validation is not expected for this file
            if (!confirmValidationExpectedDelegate.Confirm(sourceUriSansSession)) return;

            if (fileController.Exists(destinationUri))
            {
                await statusController.InformAsync(status, "Validation file already exists, will not be redownloading");
                return;
            }

            var validationSourceUri = formatValidationUriDelegate.Format(sourceUri);

            var downloadValidationFileTask = await statusController.CreateAsync(status, "Download validation file");

            await downloadFileFromSourceAsyncDelegate.DownloadFileFromSourceAsync(
                validationSourceUri,
                validationDirectoryDelegate.GetDirectory(),
                downloadValidationFileTask);

            await statusController.CompleteAsync(downloadValidationFileTask);
        }
    }
}
