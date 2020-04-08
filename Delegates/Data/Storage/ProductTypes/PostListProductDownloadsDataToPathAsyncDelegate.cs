using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        PostListProductDownloadsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.PostListProductDownloadsDataAsyncDelegate,Delegates",
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