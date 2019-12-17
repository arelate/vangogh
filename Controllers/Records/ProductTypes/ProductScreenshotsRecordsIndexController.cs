using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductScreenshotsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductScreenshotsRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductScreenshotsRecordsIndexController(
            IDataController<ProductRecords> productScreenshotsRecordsController,
            IStatusController statusController) :
            base(
                productScreenshotsRecordsController,
                statusController)
        {
            // ...
        }
    }
}