using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class GameProductDataRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
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