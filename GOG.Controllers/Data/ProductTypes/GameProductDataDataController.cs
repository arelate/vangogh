using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

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
            "Controllers.Status.StatusController,Controllers")]
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