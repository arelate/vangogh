using System.Threading.Tasks;

using Interfaces.Validation;
using Interfaces.UriResolution;
using Interfaces.Destination;
using Interfaces.Download;

namespace Controllers.Validation
{
    public class ValidationDataDownloadController : IValidationDataDownloadController
    {
        private IUriResolutionController validationUriResolutionController;
        private IDestinationController validationDestinationController;
        private IDownloadController downloadController;

        public ValidationDataDownloadController(
            IUriResolutionController validationUriResolutionController,
            IDestinationController validationDestinationController,
            IDownloadController downloadController)
        {
            this.validationUriResolutionController = validationUriResolutionController;
            this.validationDestinationController = validationDestinationController;
            this.downloadController = downloadController;
        }

        public async Task DownloadValidationData(string uri)
        {
            var validationFileSourceUri = validationUriResolutionController.ResolveUri(uri);

            await downloadController.DownloadFile(
                validationFileSourceUri, 
                validationDestinationController.GetDirectory(string.Empty));
        }
    }
}
