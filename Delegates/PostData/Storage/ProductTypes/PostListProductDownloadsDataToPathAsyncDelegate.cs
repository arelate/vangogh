using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.ProductTypes
{
    public class PostListProductDownloadsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostListProductDownloadsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetProductDownloadsPathDelegate,Delegates")]
        public PostListProductDownloadsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductDownloads>> postListProductDownloadsDataAsyncDelegate,
            IGetPathDelegate getProductDownloadsPathDelegate) :
            base(
                postListProductDownloadsDataAsyncDelegate,
                getProductDownloadsPathDelegate)
        {
            // ...
        }
    }
}