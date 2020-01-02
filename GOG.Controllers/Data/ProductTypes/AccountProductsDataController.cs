using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class AccountProductsDataController : DataController<AccountProduct>
    {
        [Dependencies(
            "GOG.Controllers.Stash.ProductTypes.AccountProductsStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertAccountProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.AccountProductsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public AccountProductsDataController(
            IStashController<List<AccountProduct>> accountProductsStashController,
            IConvertDelegate<AccountProduct, long> convertAccountProductsToIndexDelegate,
            IRecordsController<long> accountProductsRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                accountProductsStashController,
                convertAccountProductsToIndexDelegate,
                accountProductsRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}