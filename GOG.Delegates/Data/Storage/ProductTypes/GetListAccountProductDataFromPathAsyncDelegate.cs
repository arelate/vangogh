using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListAccountProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataAsyncDelegate),
            typeof(GetAccountProductsPathDelegate))]
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