using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductScreenshotsDataController : DataController<ProductScreenshots>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.ProductScreenshotsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductScreenshotsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductScreenshotsDataController(
            IStashController<List<ProductScreenshots>> productScreenshotsStashController,
            IConvertDelegate<ProductScreenshots, long> convertProductScreenshotsToIndexDelegate,
            IRecordsController<long> productScreenshotsRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                productScreenshotsStashController,
                convertProductScreenshotsToIndexDelegate,
                productScreenshotsRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}