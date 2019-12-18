using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductScreenshotsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductScreenshotsRecordsDataController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProductScreenshotsRecordsIndexController(
            IDataController<ProductRecords> productScreenshotsRecordsController,
            IActionLogController actionLogController) :
            base(
                productScreenshotsRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}