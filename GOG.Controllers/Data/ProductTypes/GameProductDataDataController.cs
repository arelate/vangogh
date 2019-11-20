using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameProductDataDataController : DataController<GameProductData>
    {
        public GameProductDataDataController(
            IStashController<Dictionary<long, GameProductData>> gameProductDataStashController,
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate,
            IRecordsController<long> gameProductDataRecordsIndexController,
            IStatusController statusController,
            params ICommitAsyncDelegate[] hashesController) :
            base(
                gameProductDataStashController,
                convertGameProductDataToIndexDelegate,
                gameProductDataRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}