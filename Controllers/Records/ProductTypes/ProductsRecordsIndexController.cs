using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductsRecordsDataController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProductsRecordsIndexController(
            IDataController<ProductRecords> productsRecordsController,
            IActionLogController actionLogController) :
            base(
                productsRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}