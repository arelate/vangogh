using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class ProductScreenshotsRecordsDataController : DataController<ProductRecords>
    {
        public ProductScreenshotsRecordsDataController(
            IStashController<Dictionary<long, ProductRecords>> productScreenshotsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                productScreenshotsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}