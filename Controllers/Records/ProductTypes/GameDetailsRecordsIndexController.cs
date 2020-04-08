using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class GameDetailsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.GameDetailsRecordsDataController,Controllers")]
        public GameDetailsRecordsIndexController(
            IDataController<ProductRecords> gameDetailsRecordsController) :
            base(
                gameDetailsRecordsController)
        {
            // ...
        }
    }
}