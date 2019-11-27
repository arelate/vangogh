using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class ProductDownloadsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductDownloadsRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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