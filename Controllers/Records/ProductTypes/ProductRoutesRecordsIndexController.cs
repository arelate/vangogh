using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductRoutesRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductRoutesRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductRoutesRecordsIndexController(
            IDataController<ProductRecords> productRoutesRecordsController,
            IActionLogController actionLogController) :
            base(
                productRoutesRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}