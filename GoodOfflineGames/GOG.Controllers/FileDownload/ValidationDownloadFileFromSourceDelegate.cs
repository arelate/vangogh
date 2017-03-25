using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Expectation;
using Interfaces.Enumeration;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Uri;
using Interfaces.File;
using Interfaces.FileDownload;
using Interfaces.TaskStatus;

namespace GOG.Controllers.FileDownload
{
    public class ValidationDownloadFileFromSourceDelegate : IDownloadFileFromSourceDelegate
    {
        private IStringExtractionController uriSansSessionExtractionController;
        private IExpectedDelegate<string> validationExpectedForUriDelegate;
        private IEnumerateDelegate<string> validationFileEnumerateDelegate;
        private IGetDirectoryDelegate validationDirectoryDelegate;
        private IGetUriDelegate validationUriDelegate;
        private IFileController fileController;
        private IFileDownloadController fileDownloadController;
        private ITaskStatusController taskStatusController;

        public ValidationDownloadFileFromSourceDelegate(
            IStringExtractionController uriSansSessionExtractionController,
            IExpectedDelegate<string> validationExpectedForUriDelegate,
            IEnumerateDelegate<string> validationFileEnumerateDelegate,
            IGetDirectoryDelegate validationDirectoryDelegate,
            IGetUriDelegate validationUriDelegate,
            IFileController fileController,
            IFileDownloadController fileDownloadController,
            ITaskStatusController taskStatusController)
        {
            this.uriSansSessionExtractionController = uriSansSessionExtractionController;
            this.validationExpectedForUriDelegate = validationExpectedForUriDelegate;
            this.validationFileEnumerateDelegate = validationFileEnumerateDelegate;
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.validationUriDelegate = validationUriDelegate;
            this.fileController = fileController;
            this.fileDownloadController = fileDownloadController;
            this.taskStatusController = taskStatusController;
        }

        public async Task DownloadFileFromSourceAsync(long id, string title, string sourceUri, string destination, ITaskStatus taskStatus)
        {
            if (string.IsNullOrEmpty(sourceUri)) return;

            var sourceUriSansSession = uriSansSessionExtractionController.ExtractMultiple(sourceUri).First();
            var destinationUri = validationFileEnumerateDelegate.Enumerate(sourceUriSansSession).First();
            
            // return early if validation is not expected for this file
            if (!validationExpectedForUriDelegate.Expected(sourceUriSansSession)) return;

            if (fileController.Exists(destinationUri))
            {
                taskStatusController.Inform(taskStatus, "Validation file already exists, will not be redownloading");
                return;
            }

            var validationSourceUri = validationUriDelegate.GetUri(sourceUri);

            var downloadValidationFileTask = taskStatusController.Create(taskStatus, "Download validation file");

            await fileDownloadController.DownloadFileFromSourceAsync(
                id,
                title,
                validationSourceUri,
                validationDirectoryDelegate.GetDirectory(),
                downloadValidationFileTask);

            taskStatusController.Complete(downloadValidationFileTask);
        }
    }
}
