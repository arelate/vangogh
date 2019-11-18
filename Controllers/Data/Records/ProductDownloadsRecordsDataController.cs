using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class ProductDownloadsRecordsDataController : DataController<ProductRecords>
    {
        public ProductDownloadsRecordsDataController(
            IStashController<Dictionary<long, ProductRecords>> productDownloadsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                productDownloadsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}