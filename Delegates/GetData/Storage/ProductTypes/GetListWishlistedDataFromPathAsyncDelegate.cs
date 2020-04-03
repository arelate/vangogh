using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListWishlistedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListWishlistedDataAsyncDelegate,Delegates",
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