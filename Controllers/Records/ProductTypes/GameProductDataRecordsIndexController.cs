using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class GameProductDataRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.GameProductDataRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameProductDataRecordsIndexController(
            IDataController<ProductRecords> gameProductDataRecordsController,
            IActionLogController actionLogController) :
            base(
                gameProductDataRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}