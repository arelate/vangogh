using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class AccountProductsDataController : DataController<AccountProduct>
    {
        public AccountProductsDataController(
            IStashController<Dictionary<long, AccountProduct>> accountProductsStashController,
            IConvertDelegate<AccountProduct, long> convertAccountProductsToIndexDelegate,
            IRecordsController<long> accountProductsRecordsIndexController,
            IStatusController statusController,
            params ICommitAsyncDelegate[] hashesController) :
            base(
                accountProductsStashController,
                convertAccountProductsToIndexDelegate,
                accountProductsRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}