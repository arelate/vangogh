using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.Data
{
    public class ProductDownloadsRecordsIndexController : IndexRecordsController
    {
        public ProductDownloadsRecordsIndexController(
            IDataController<ProductRecords> productDownloadsRecordsController,
            IStatusController statusController) :
            base(
                productDownloadsRecordsController,
                statusController)
        {
            // ...
        }
    }
}