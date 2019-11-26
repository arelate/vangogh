using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class WishlistedRecordsIndexController : IndexRecordsController
    {
        public WishlistedRecordsIndexController(
            IDataController<ProductRecords> wishlistedRecordsController,
            IStatusController statusController) :
            base(
                wishlistedRecordsController,
                statusController)
        {
            // ...
        }
    }
}