using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListAccountProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetAccountProductsPathDelegate,Delegates")]
        public GetListAccountProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>,string> getListAccountProductDataAsyncDelegate,
            IGetPathDelegate getAccountProductsPathDelegate) :
            base(
                getListAccountProductDataAsyncDelegate,
                getAccountProductsPathDelegate)
        {
            // ...
        }
    }
}