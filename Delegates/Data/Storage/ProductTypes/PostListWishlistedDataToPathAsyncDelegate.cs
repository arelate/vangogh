using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListWishlistedDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(PostListWishlistedDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate))]
        public PostListWishlistedDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<long>> postListWishlistedDataAsyncDelegate,
            IGetPathDelegate getWishlistedPathDelegate) :
            base(
                postListWishlistedDataAsyncDelegate,
                getWishlistedPathDelegate)
        {
            // ...
        }
    }
}