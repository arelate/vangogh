using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class ProductDownloadsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Controllers.Stash.Records.ProductDownloadsRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductDownloadsRecordsDataController(
            IStashController<List<ProductRecords>> productDownloadsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                productDownloadsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}