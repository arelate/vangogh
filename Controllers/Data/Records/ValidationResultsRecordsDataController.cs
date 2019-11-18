using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class ValidationResultsRecordsDataController : DataController<ProductRecords>
    {
        public ValidationResultsRecordsDataController(
            IStashController<Dictionary<long, ProductRecords>> validationResultsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                validationResultsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}