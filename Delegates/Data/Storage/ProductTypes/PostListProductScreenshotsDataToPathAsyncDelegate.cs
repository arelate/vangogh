using System.Collections.Generic;
using Attributes;
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
            typeof(Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate))]
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