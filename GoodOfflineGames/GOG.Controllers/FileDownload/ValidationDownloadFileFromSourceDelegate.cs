using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Expectation;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
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
        private IGetDirectoryDelegate getValidationDirectoryDelegate;
        private IGetFilenameDelegate getValidationFilenameDelegate;
        private IGetUriDelegate getValidationUriDelegate;
        private IFileController fileController;
        private IFileDownloadController fileDownloadController;
        private ITaskStatusController taskStatusController;

        public ValidationDownloadFileFromSourceDelegate(
            IStringExtractionController uriSansSessionExtractionController,
            IExpectedDelegate<string> validationExpectedForUriDelegate,
            IGetDirectoryDelegate getValidationDirectoryDelegate,
            IGetFilenameDelegate getValidationFilenameDelegate,
            IGetUriDelegate getValidationUriDelegate,
            IFileController fileController,
            IFileDownloadController fileDownloadController,
            ITaskStatusController taskStatusController)
        {
            this.uriSansSessionExtractionController = uriSansSessionExtractionController;
            this.validationExpectedForUriDelegate = validationExpectedForUriDelegate;
            this.getValidationDirectoryDelegate = getValidationDirectoryDelegate;
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;
            this.getValidationUriDelegate = getValidationUriDelegate;
            this.fileController = fileController;
            this.fileDownloadController = fileDownloadController;
            this.taskStatusController = taskStatusController;
        }

        public async Task DownloadFileFromSourceAsync(long id, string title, string sourceUri, string destination, ITaskStatus taskStatus)
        {
            if (string.IsNullOrEmpty(sourceUri)) return;

            var sourceUriSansSession = uriSansSessionExtractionController.ExtractMultiple(sourceUri).First();
            var validationDirectory = getValidationDirectoryDelegate.GetDirectory();

            var destinationUri = Path.Combine(
                validationDirectory,
                getValidationFilenameDelegate.GetFilename(Path.GetFileName(sourceUriSansSession)));

            // return early if validation is not expected for this file
            if (!validationExpectedForUriDelegate.Expected(sourceUriSansSession)) return;

            if (fileController.Exists(destinationUri))
            {
                taskStatusController.Inform(taskStatus, "Validation file already exists, will not be redownloading");
                return;
            }

            var validationSourceUri = getValidationUriDelegate.GetUri(sourceUri);

            var downloadValidationFileTask = taskStatusController.Create(taskStatus, "Download validation file");

            await fileDownloadController.DownloadFileFromSourceAsync(
                id, 
                title, 
                validationSourceUri, 
                validationDirectory, 
                downloadValidationFileTask);

            taskStatusController.Complete(downloadValidationFileTask);

            throw new NotImplementedException();
        }
    }
}
