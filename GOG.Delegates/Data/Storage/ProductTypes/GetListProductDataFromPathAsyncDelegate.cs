using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(GetListProductDataAsyncDelegate),
            typeof(GetProductsPathDelegate))]
        public GetListProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<Product>,string> getListProductDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductsPathDelegate) :
            base(
                getListProductDataAsyncDelegate,
                getProductsPathDelegate)
        {
            // ...
        }
    }
}