using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.ProductTypes
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