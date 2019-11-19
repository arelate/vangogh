using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.Data
{
    public class ProductsRecordsIndexController : IndexRecordsController
    {
        public ProductsRecordsIndexController(
            IDataController<ProductRecords> productsRecordsController,
            IStatusController statusController) :
            base(
                productsRecordsController,
                statusController)
        {
            // ...
        }
    }
}