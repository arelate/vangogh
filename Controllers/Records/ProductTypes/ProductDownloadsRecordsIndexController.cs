using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductDownloadsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Data.Records.ProductDownloadsRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
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