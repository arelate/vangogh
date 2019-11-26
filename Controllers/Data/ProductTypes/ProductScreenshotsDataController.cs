using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.ProductScreenshots;

namespace Controllers.Data.ProductTypes
{
    public class ProductScreenshotsDataController : DataController<ProductScreenshots>
    {
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