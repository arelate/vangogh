using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Download;

using Interfaces.Controllers.File;

using Interfaces.Status;

using Attributes;

using GOG.Interfaces.Delegates.DownloadProductFile;

namespace GOG.Delegates.DownloadProductFile
{
    public class DownloadValidationFileAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        readonly IFormatDelegate<string, string> formatUriRemoveSessionDelegate;
        readonly IConfirmDelegate<string> confirmValidationExpectedDelegate;
        readonly IFormatDelegate<string, string> formatValidationFileDelegate;
        readonly IGetDirectoryDelegate validationDirectoryDelegate;
        readonly IFormatDelegate<string, string> formatValidationUriDelegate;
        readonly IFileController fileController;
        readonly IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;
        readonly IStatusController statusController;

		[Dependencies(
			"Delegates.Format.Uri.FormatUriRemoveSessionDelegate,Delegates",
			"Delegates.Confirm.ConfirmValidationExpectedDelegate,Delegates",
			"Delegates.Format.Uri.FormatValidationFileDelegate,Delegates",
			"Delegates.GetDirectory.ProductTypes.GetMd5DirectoryDelegate,Delegates",
			"Delegates.Format.Uri.FormatValidationUriDelegate,Delegates",
			"Controllers.File.FileController,Controllers",
			"Delegates.Download.DownloadFromUriAsyncDelegate,Delegates",
			"Controllers.Status.StatusController,Controllers")]
        public DownloadValidationFileAsyncDelegate(
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate,
            IConfirmDelegate<string> confirmValidationExpectedDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IGetDirectoryDelegate validationDirectoryDelegate,
            IFormatDelegate<string, string> formatValidationUriDelegate,
            IFileController fileController,
            IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate,
            IStatusController statusController)
        {
            this.formatUriRemoveSessionDelegate = formatUriRemoveSessionDelegate;
            this.confirmValidationExpectedDelegate = confirmValidationExpectedDelegate;
            this.formatValidationFileDelegate = formatValidationFileDelegate;
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.formatValidationUriDelegate = formatValidationUriDelegate;
            this.fileController = fileController;
            this.downloadFromUriAsyncDelegate = downloadFromUriAsyncDelegate;
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

            await downloadFromUriAsyncDelegate.DownloadFromUriAsync(
                validationSourceUri,
                validationDirectoryDelegate.GetDirectory(string.Empty),
                downloadValidationFileTask);

            await statusController.CompleteAsync(downloadValidationFileTask);
        }
    }
}
