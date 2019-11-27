using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class ApiProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ApiProductsRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ApiProductsRecordsIndexController(
            IDataController<ProductRecords> apiProductsRecordsController,
            IStatusController statusController) :
            base(
                apiProductsRecordsController,
                statusController)
        {
            // ...
        }
    }
}