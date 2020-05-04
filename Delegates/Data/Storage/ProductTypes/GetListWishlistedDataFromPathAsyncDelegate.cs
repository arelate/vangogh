using System.Collections.Generic;
using Attributes;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListWishlistedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(GetListWishlistedDataAsyncDelegate),
            typeof(GetWishlistedPathDelegate))]
        public GetListWishlistedDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getListWishlistedDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getWishlistedPathDelegate) :
            base(
                getListWishlistedDataAsyncDelegate,
                getWishlistedPathDelegate)
        {
            // ...
        }
    }
}