using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class GameDetailsRecordsIndexController : IndexRecordsController
    {
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