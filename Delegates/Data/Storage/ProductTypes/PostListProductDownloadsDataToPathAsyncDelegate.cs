using System.Collections.Generic;
using Attributes;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        PostListProductDownloadsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            typeof(PostListProductDownloadsDataAsyncDelegate),
            typeof(GetProductDownloadsPathDelegate))]
        public PostListProductDownloadsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductDownloads>> postListProductDownloadsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductDownloadsPathDelegate) :
            base(
                postListProductDownloadsDataAsyncDelegate,
                getProductDownloadsPathDelegate)
        {
            // ...
        }
    }
}