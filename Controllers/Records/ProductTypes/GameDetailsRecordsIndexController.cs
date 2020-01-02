using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class GameDetailsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.GameDetailsRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameDetailsRecordsIndexController(
            IDataController<ProductRecords> gameDetailsRecordsController,
            IActionLogController actionLogController) :
            base(
                gameDetailsRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}