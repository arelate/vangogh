using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

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
            "Controllers.Status.StatusController,Controllers")]
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