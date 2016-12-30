using System;
using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.Validation;
using Interfaces.UriResolution;
using Interfaces.Destination;
using Interfaces.Network;
using Interfaces.Download;
using Interfaces.Reporting;

namespace Controllers.Validation
{
    public class ValidationDataDownloadController : IValidationDataDownloadController
    {
        private IUriResolutionController validationUriResolutionController;
        private IDestinationController validationDestinationController;
        private INetworkController networkController;
        private IDownloadController downloadController;
        private ITaskReportingController taskReportingController;

        public ValidationDataDownloadController(
            IUriResolutionController validationUriResolutionController,
            IDestinationController validationDestinationController,
            INetworkController networkController,
            IDownloadController downloadController,
            ITaskReportingController taskReportingController)
        {
            this.validationUriResolutionController = validationUriResolutionController;
            this.validationDestinationController = validationDestinationController;
            this.networkController = networkController;
            this.downloadController = downloadController;
            this.taskReportingController = taskReportingController;
        }

        public async Task DownloadValidationDataAsync(string uri)
        {
            var validationFileSourceUri = validationUriResolutionController.ResolveUri(uri);

            try
            {
                var response = await networkController.GetResponse(HttpMethod.Get, validationFileSourceUri);

                await downloadController.DownloadFileAsync(
                    response,
                    validationDestinationController.GetDirectory(string.Empty));
            }
            catch (Exception ex)
            {
                taskReportingController.ReportWarning(ex.Message);
            }
        }
    }
}
