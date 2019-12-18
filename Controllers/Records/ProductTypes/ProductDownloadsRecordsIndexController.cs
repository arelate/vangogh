using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductDownloadsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductDownloadsRecordsDataController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProductDownloadsRecordsIndexController(
            IDataController<ProductRecords> productDownloadsRecordsController,
            IActionLogController actionLogController) :
            base(
                productDownloadsRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}