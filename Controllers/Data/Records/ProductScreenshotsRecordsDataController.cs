using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class ProductScreenshotsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.Records.ProductScreenshotsRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductScreenshotsRecordsDataController(
            IStashController<List<ProductRecords>> productScreenshotsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                productScreenshotsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}