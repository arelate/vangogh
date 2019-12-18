using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using GOG.Models;

namespace GOG.Activities.DownloadProductFiles.ProductTypes
{
    public class DownloadProductFilesActivity : DownloadFilesActivity<ProductFile>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadManualUrlFileAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ResponseLogController,Controllers")]        
        public DownloadProductFilesActivity(
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IResponseLogController responseLogController) :
            base(
                productDownloadsDataController,
                downloadProductFileAsyncDelegate,
                responseLogController)
        {
            // ...
        }
    }
}
