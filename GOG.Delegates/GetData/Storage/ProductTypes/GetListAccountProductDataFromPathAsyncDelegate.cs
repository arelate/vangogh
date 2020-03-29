using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListAccountProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListAccountProductDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetAccountProductsPathDelegate,Delegates")]
        public GetListAccountProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>> getListAccountProductDataAsyncDelegate, 
            IGetPathDelegate getAccountProductsPathDelegate) : 
            base(
                getListAccountProductDataAsyncDelegate, 
                getAccountProductsPathDelegate)
        {
            // ...
        }
    }
}