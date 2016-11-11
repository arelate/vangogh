using System;
using System.Threading.Tasks;

using Interfaces.Validation;
using Interfaces.UriResolution;
using Interfaces.Destination;
using Interfaces.Download;
using Interfaces.Reporting;

namespace Controllers.Validation
{
    public class ValidationDataDownloadController : IValidationDataDownloadController
    {
        private IUriResolutionController validationUriResolutionController;
        private IDestinationController validationDestinationController;
        private IDownloadController downloadController;
        private ITaskReportingController taskReportingController;

        public ValidationDataDownloadController(
            IUriResolutionController validationUriResolutionController,
            IDestinationController validationDestinationController,
            IDownloadController downloadController,
            ITaskReportingController taskReportingController)
        {
            this.validationUriResolutionController = validationUriResolutionController;
            this.validationDestinationController = validationDestinationController;
            this.downloadController = downloadController;
            this.taskReportingController = taskReportingController;
        }

        public async Task DownloadValidationData(string uri)
        {
            var validationFileSourceUri = validationUriResolutionController.ResolveUri(uri);

            try
            {
                await downloadController.DownloadFile(
                    validationFileSourceUri,
                    validationDestinationController.GetDirectory(string.Empty));
            }
            catch (Exception ex)
            {
                taskReportingController.ReportWarning(ex.Message);
            }
        }
    }
}
