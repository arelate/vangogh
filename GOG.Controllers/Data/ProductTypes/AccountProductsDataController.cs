using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class AccountProductsDataController : DataController<AccountProduct>
    {
        public AccountProductsDataController(
            IStashController<List<AccountProduct>> accountProductsStashController,
            IConvertDelegate<AccountProduct, long> convertAccountProductsToIndexDelegate,
            IRecordsController<long> accountProductsRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                accountProductsStashController,
                convertAccountProductsToIndexDelegate,
                accountProductsRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}