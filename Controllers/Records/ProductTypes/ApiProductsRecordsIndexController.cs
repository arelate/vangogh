using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ApiProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ApiProductsRecordsDataController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ApiProductsRecordsIndexController(
            IDataController<ProductRecords> apiProductsRecordsController,
            IActionLogController actionLogController) :
            base(
                apiProductsRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}