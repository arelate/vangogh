using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListWishlistedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListWishlistedDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate,Delegates")]
        public GetListWishlistedDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>> getListWishlistedDataAsyncDelegate, 
            IGetPathDelegate getWishlistedPathDelegate) : 
            base(
                getListWishlistedDataAsyncDelegate, 
                getWishlistedPathDelegate)
        {
            // ...
        }
    }
}