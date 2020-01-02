using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameProductDataDataController : DataController<GameProductData>
    {
        [Dependencies(
            "GOG.Controllers.Stash.ProductTypes.GameProductDataStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameProductDataToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.GameProductDataRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameProductDataDataController(
            IStashController<List<GameProductData>> gameProductDataStashController,
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate,
            IRecordsController<long> gameProductDataRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                gameProductDataStashController,
                convertGameProductDataToIndexDelegate,
                gameProductDataRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}