using System.IO;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Validation;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Destination;

using GOG.TaskActivities.Abstract;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Validation
{
    public class ProcessValidationController : TaskActivityController
    {
        private IDestinationController destinationController;
        private IValidationController validationController;
        private IProductTypeStorageController productTypeStorageController;

        public ProcessValidationController(
            IDestinationController destinationController,
            IValidationController validationController,
            IProductTypeStorageController productTypeStorageController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.destinationController = destinationController;
            this.validationController = validationController;
            this.productTypeStorageController = productTypeStorageController;
        }

        public override async Task ProcessTask()
        {
            var scheduledDownloads = await productTypeStorageController.Pull<ScheduledDownload>(ProductTypes.ScheduledDownload);

            foreach (var download in scheduledDownloads)
            {
                if (download.Type != ScheduledDownloadTypes.File)
                    continue;

                var filename = Path.Combine(
                    download.Destination,
                    destinationController.GetFilename(download.Source));

                await validationController.Validate(filename);
            }
        }
    }
}
