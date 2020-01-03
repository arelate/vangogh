using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ApiProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Data.Records.ApiProductsRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
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