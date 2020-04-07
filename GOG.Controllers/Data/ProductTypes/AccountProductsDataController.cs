using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Delegates.Activities;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class AccountProductsDataController : DataController<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListAccountProductDataToPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertAccountProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.AccountProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Collections.ProductTypes.FindAccountProductDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public AccountProductsDataController(
            IGetDataAsyncDelegate<List<AccountProduct>> getListAccountProductDataAsyncDelegate,
            IPostDataAsyncDelegate<List<AccountProduct>> postListAccountProductDataAsyncDelegate,
            IConvertDelegate<AccountProduct, long> convertAccountProductsToIndexDelegate,
            IRecordsController<long> accountProductsRecordsIndexController,
            IFindDelegate<AccountProduct> findAccountProductDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getListAccountProductDataAsyncDelegate,
                postListAccountProductDataAsyncDelegate,
                convertAccountProductsToIndexDelegate,
                accountProductsRecordsIndexController,
                findAccountProductDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}