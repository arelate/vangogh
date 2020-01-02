using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class GameProductDataRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Controllers.Stash.Records.GameProductDataRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameProductDataRecordsDataController(
            IStashController<List<ProductRecords>> gameProductDataRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                gameProductDataRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}