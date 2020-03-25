using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;


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
            "GOG.Delegates.Find.ProductTypes.FindAccountProductDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public AccountProductsDataController(
            IStashController<List<AccountProduct>> accountProductsStashController,
            IConvertDelegate<AccountProduct, long> convertAccountProductsToIndexDelegate,
            IRecordsController<long> accountProductsRecordsIndexController,
            IFindDelegate<AccountProduct> findAccountProductDelegate,
            IActionLogController actionLogController) :
            base(
                accountProductsStashController,
                convertAccountProductsToIndexDelegate,
                accountProductsRecordsIndexController,
                findAccountProductDelegate,
                actionLogController)
        {
            // ...
        }
    }
}