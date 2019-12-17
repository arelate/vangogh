using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class WishlistedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.WishlistedRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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