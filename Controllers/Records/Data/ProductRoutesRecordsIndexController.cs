using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.Data
{
    public class ProductRoutesRecordsIndexController : IndexRecordsController
    {
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