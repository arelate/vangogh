using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListAccountProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            typeof(GetListAccountProductDataAsyncDelegate),
            typeof(GetAccountProductsPathDelegate))]
        public GetListAccountProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>,string> getListAccountProductDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getAccountProductsPathDelegate) :
            base(
                getListAccountProductDataAsyncDelegate,
                getAccountProductsPathDelegate)
        {
            // ...
        }
    }
}