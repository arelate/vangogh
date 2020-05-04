using System.IO;
using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Download;
using Interfaces.Delegates.Activities;
using Attributes;
using Delegates.Format.Uri;
using Delegates.Confirm.Validation;
using Delegates.Download;
using Delegates.Activities;
using Delegates.Values.Directories.ProductTypes;
using GOG.Models;

namespace GOG.Delegates.Data.Models
{
    public class GetValidationFileAsyncDelegate : IGetDataAsyncDelegate<string, ProductFileDownloadManifest>
    {
        private readonly IFormatDelegate<string, string> formatUriRemoveSessionDelegate;
        private readonly IConfirmDelegate<string> confirmValidationExpectedDelegate;
        private readonly IFormatDelegate<string, string> formatValidationFileDelegate;
        private readonly IGetValueDelegate<string,string> validationDirectoryDelegate;
        private readonly IFormatDelegate<string, string> formatValidationUriDelegate;
        private readonly IDownloadFromUriAsyncDelegate downloadFromUriAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(FormatUriRemoveSessionDelegate),
            typeof(ConfirmFileValidationSupportedDelegate),
            typeof(FormatValidationFileDelegate),
            typeof(GetMd5DirectoryDelegate),
            typeof(FormatValidationUriDelegate),
            typeof(DownloadFromUriAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public GetValidationFileAsyncDelegate(
            IFormatDelegate<string, string> formatUriRemoveSessionDelegate,
            IConfirmDelegate<string> confirmValidationExpectedDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IGetValueDelegate<string,string> validationDirectoryDelegate,
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

        public async Task<string> GetDataAsync(ProductFileDownloadManifest downloadManifest)
        {
            if (string.IsNullOrEmpty(downloadManifest.Source)) return string.Empty;

            var sourceUriSansSession = formatUriRemoveSessionDelegate.Format(downloadManifest.Source);
            var destinationUri = formatValidationFileDelegate.Format(sourceUriSansSession);

            // return early if validation is not expected for this file
            if (!confirmValidationExpectedDelegate.Confirm(sourceUriSansSession)) return string.Empty;

            if (File.Exists(destinationUri))
                // await statusController.InformAsync(status, "Validation file already exists, will not be redownloading");
                return string.Empty;

            var validationSourceUri = formatValidationUriDelegate.Format(downloadManifest.Source);

            startDelegate.Start("Download validation file");

            await downloadFromUriAsyncDelegate.DownloadFromUriAsync(
                validationSourceUri,
                validationDirectoryDelegate.GetValue(string.Empty));

            completeDelegate.Complete();

            return downloadManifest.Destination;
        }
    }
}