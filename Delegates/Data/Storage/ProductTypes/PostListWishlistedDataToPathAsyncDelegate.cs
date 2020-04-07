using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListWishlistedDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.PostListWishlistedDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate,Delegates")]
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