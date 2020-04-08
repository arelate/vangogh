using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        PostListProductScreenshotsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.PostListProductScreenshotsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate,Delegates")]
        public PostListProductScreenshotsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductScreenshots>> postListProductScreenshotsDataAsyncDelegate,
            IGetPathDelegate getListProductScreenshotsPathDelegate) :
            base(
                postListProductScreenshotsDataAsyncDelegate,
                getListProductScreenshotsPathDelegate)
        {
            // ...
        }
    }
}