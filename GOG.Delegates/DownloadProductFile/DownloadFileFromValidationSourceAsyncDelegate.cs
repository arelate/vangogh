using System.IO;
using System.Threading.Tasks;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Download;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Interfaces.Delegates.DownloadProductFile;

namespace GOG.Delegates.DownloadProductFile
{
    public class DownloadValidationFileAsyncDelegate : IDownloadProductFileAsyncDelegate
    {
        private readonly IFormatDelegate<string, string> formatUriRemoveSessionDelegate;
        private readonly IConfirmDelegate<string> confirmValidationExpectedDelegate;
        private readonly IFormatDelegate<string, string> formatValidationFileDelegate;
        private readonly IGetDirectoryDelegate validationDirectoryDelegate;
        private readonly IFormatDelegate<string, string> formatValidationUriDelegate;
        private readonly IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "Delegates.Format.Uri.FormatUriRemoveSessionDelegate,Delegates",
            "Delegates.Confirm.Validation.ConfirmFileValidationSupportedDelegate,Delegates",
            "Delegates.Format.Uri.FormatValidationFileDelegate,Delegates",
            "Delegates.GetDirectory.ProductTypes.GetMd5DirectoryDelegate,Delegates",
            "Delegates.Format.Uri.FormatValidationUriDelegate,Delegates",
            "Delegates.Download.DownloadFromUriAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public DownloadValidationFileAsyncDelegate(
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate,
            IConfirmDelegate<string> confirmValidationExpectedDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IGetDirectoryDelegate validationDirectoryDelegate,
            IFormatDelegate<string, string> formatValidationUriDelegate,
            IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.formatUriRemoveSessionDelegate = formatUriRemoveSessionDelegate;
            this.confirmValidationExpectedDelegate = confirmValidationExpectedDelegate;
            this.formatValidationFileDelegate = formatValidationFileDelegate;
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.formatValidationUriDelegate = formatValidationUriDelegate;
            this.downloadFromUriAsyncDelegate = downloadFromUriAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task DownloadProductFileAsync(long id, string title, string sourceUri, string destination)
        {
            if (string.IsNullOrEmpty(sourceUri)) return;

            var sourceUriSansSession = formatUriRemoveSessionDelegate.Format(sourceUri);
            var destinationUri = formatValidationFileDelegate.Format(sourceUriSansSession);

            // return early if validation is not expected for this file
            if (!confirmValidationExpectedDelegate.Confirm(sourceUriSansSession)) return;

            if (File.Exists(destinationUri))
                // await statusController.InformAsync(status, "Validation file already exists, will not be redownloading");
                return;

            var validationSourceUri = formatValidationUriDelegate.Format(sourceUri);

            startDelegate.Start("Download validation file");

            await downloadFromUriAsyncDelegate.DownloadFromUriAsync(
                validationSourceUri,
                validationDirectoryDelegate.GetDirectory(string.Empty));

            completeDelegate.Complete();
        }
    }
}