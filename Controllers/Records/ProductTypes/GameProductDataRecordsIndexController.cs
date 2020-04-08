using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class GameProductDataRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.GameProductDataRecordsDataController,Controllers")]
        public GameProductDataRecordsIndexController(
            IDataController<ProductRecords> gameProductDataRecordsController) :
            base(
                gameProductDataRecordsController)
        {
            // ...
        }
    }
}