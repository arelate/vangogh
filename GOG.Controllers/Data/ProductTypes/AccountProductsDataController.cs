using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class AccountProductsDataController : DataController<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListAccountProductDataToPathAsyncDelegate,GOG.Delegate",
            "GOG.Delegates.Convert.ProductTypes.ConvertAccountProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.AccountProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindAccountProductDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public AccountProductsDataController(
            IGetDataAsyncDelegate<List<AccountProduct>> getListAccountProductDataAsyncDelegate,
            IPostDataAsyncDelegate<List<AccountProduct>> postListAccountProductDataAsyncDelegate,
            IConvertDelegate<AccountProduct, long> convertAccountProductsToIndexDelegate,
            IRecordsController<long> accountProductsRecordsIndexController,
            IFindDelegate<AccountProduct> findAccountProductDelegate,
            IActionLogController actionLogController) :
            base(
                getListAccountProductDataAsyncDelegate,
                postListAccountProductDataAsyncDelegate,
                convertAccountProductsToIndexDelegate,
                accountProductsRecordsIndexController,
                findAccountProductDelegate,
                actionLogController)
        {
            // ...
        }
    }
}