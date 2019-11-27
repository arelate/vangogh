using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ProductScreenshots;

namespace Controllers.Data.ProductTypes
{
    public class ProductScreenshotsDataController : DataController<ProductScreenshots>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.ProductScreenshotsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductScreenshotsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductScreenshotsDataController(
            IStashController<List<ProductScreenshots>> productScreenshotsStashController,
            IConvertDelegate<ProductScreenshots, long> convertProductScreenshotsToIndexDelegate,
            IRecordsController<long> productScreenshotsRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                productScreenshotsStashController,
                convertProductScreenshotsToIndexDelegate,
                productScreenshotsRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}