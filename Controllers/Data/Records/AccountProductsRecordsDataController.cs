using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class AccountProductsRecordsDataController : DataController<ProductRecords>
    {
        public AccountProductsRecordsDataController(
            IStashController<Dictionary<long, ProductRecords>> accountProductsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IStatusController statusController) :
            base(
                accountProductsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                statusController)
        {
            // ...
        }
    }
}