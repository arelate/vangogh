using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataAsyncDelegate),
            typeof(GetProductsPathDelegate))]
        public GetListProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<Product>,string> getListProductDataAsyncDelegate,
            IGetPathDelegate getProductsPathDelegate) :
            base(
                getListProductDataAsyncDelegate,
                getProductsPathDelegate)
        {
            // ...
        }
    }
}