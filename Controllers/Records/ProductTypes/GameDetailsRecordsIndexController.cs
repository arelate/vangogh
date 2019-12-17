using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class GameDetailsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.GameDetailsRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public GameDetailsRecordsIndexController(
            IDataController<ProductRecords> gameDetailsRecordsController,
            IStatusController statusController) :
            base(
                gameDetailsRecordsController,
                statusController)
        {
            // ...
        }
    }
}