using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class ProductRoutesRecordsDataController : DataController<ProductRecords>
    {
        public ProductRoutesRecordsDataController(
            IStashController<Dictionary<long, ProductRecords>> productRoutesRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                productRoutesRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}