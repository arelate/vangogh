using System.Collections.Generic;

using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetProductsPathDelegate,Delegates")]
        public GetListProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<Product>> getListProductDataAsyncDelegate, 
            IGetPathDelegate getProductsPathDelegate) : 
            base(
                getListProductDataAsyncDelegate, 
                getProductsPathDelegate)
        {
            // ...
        }
    }
}