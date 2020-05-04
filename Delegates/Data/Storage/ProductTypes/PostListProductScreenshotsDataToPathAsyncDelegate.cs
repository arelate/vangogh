using System.Collections.Generic;
using Attributes;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        PostListProductScreenshotsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            typeof(PostListProductScreenshotsDataAsyncDelegate),
            typeof(GetProductScreenshotsPathDelegate))]
        public PostListProductScreenshotsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductScreenshots>> postListProductScreenshotsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getListProductScreenshotsPathDelegate) :
            base(
                postListProductScreenshotsDataAsyncDelegate,
                getListProductScreenshotsPathDelegate)
        {
            // ...
        }
    }
}