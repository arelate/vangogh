using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class ProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductsRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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