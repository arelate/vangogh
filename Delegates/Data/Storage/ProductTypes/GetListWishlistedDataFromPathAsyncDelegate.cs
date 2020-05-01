using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListWishlistedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(GetListWishlistedDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate))]
        public GetListWishlistedDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getListWishlistedDataAsyncDelegate,
            IGetPathDelegate getWishlistedPathDelegate) :
            base(
                getListWishlistedDataAsyncDelegate,
                getWishlistedPathDelegate)
        {
            // ...
        }
    }
}