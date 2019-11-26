using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameProductDataDataController : DataController<GameProductData>
    {
        public GameProductDataDataController(
            IStashController<List<GameProductData>> gameProductDataStashController,
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate,
            IRecordsController<long> gameProductDataRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                gameProductDataStashController,
                convertGameProductDataToIndexDelegate,
                gameProductDataRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}