using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.ProductScreenshots;

namespace Controllers.Data.ProductTypes
{
    public class ProductScreenshotsDataController : DataController<ProductScreenshots>
    {
        public ProductScreenshotsDataController(
            IStashController<Dictionary<long, ProductScreenshots>> productScreenshotsStashController,
            IConvertDelegate<ProductScreenshots, long> convertProductScreenshotsToIndexDelegate,
            IRecordsController<long> productScreenshotsRecordsIndexController,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                productScreenshotsStashController,
                convertProductScreenshotsToIndexDelegate,
                productScreenshotsRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}