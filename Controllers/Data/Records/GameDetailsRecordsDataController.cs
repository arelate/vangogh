using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class GameDetailsRecordsDataController : DataController<ProductRecords>
    {
        public GameDetailsRecordsDataController(
            IStashController<Dictionary<long, ProductRecords>> gameDetailsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                gameDetailsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}