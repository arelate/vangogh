using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

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