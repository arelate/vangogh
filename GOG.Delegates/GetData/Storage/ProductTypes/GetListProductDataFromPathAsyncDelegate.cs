using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductDataAsyncDelegate,Delegates",
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