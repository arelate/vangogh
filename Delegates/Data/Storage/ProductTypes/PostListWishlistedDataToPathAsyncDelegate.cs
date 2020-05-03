using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListWishlistedDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(PostListWishlistedDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate))]
        public PostListWishlistedDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<long>> postListWishlistedDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getWishlistedPathDelegate) :
            base(
                postListWishlistedDataAsyncDelegate,
                getWishlistedPathDelegate)
        {
            // ...
        }
    }
}