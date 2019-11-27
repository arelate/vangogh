using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class GameProductDataRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.GameProductDataRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public GameProductDataRecordsIndexController(
            IDataController<ProductRecords> gameProductDataRecordsController,
            IStatusController statusController) :
            base(
                gameProductDataRecordsController,
                statusController)
        {
            // ...
        }
    }
}