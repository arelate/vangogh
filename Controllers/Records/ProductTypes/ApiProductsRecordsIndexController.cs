using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class ApiProductsRecordsIndexController : IndexRecordsController
    {
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