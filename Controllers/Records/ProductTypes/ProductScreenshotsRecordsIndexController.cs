using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductScreenshotsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Data.Records.ProductScreenshotsRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
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