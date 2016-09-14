using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.Destination;
using Interfaces.File;
using Interfaces.Reporting;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductFiles
{
    public class ProductFilesScheduleDownloadsController: ScheduleDownloadsController
    {
        public ProductFilesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IDestinationController destinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (downloadSourcesController,
                productTypeStorageController,
                collectionController,
                destinationController,
                fileController,
                taskReportingController)
        {
            downloadType = Models.Custom.ScheduledDownloadTypes.File;
            destination = "";
        }
    }
}
