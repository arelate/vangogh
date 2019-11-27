using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class ProductRoutesRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductRoutesRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductRoutesRecordsIndexController(
            IDataController<ProductRecords> productRoutesRecordsController,
            IStatusController statusController) :
            base(
                productRoutesRecordsController,
                statusController)
        {
            // ...
        }
    }
}