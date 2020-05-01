using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductScreenshotsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.PostStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.ProductTypes.ConvertListProductScreenshotsToJSONDelegate))]
        public PostListProductScreenshotsDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<List<ProductScreenshots>, string> convertListProductScreenshotsToJSONDelegate) :
            base(
                postStringDataAsyncDelegate,
                convertListProductScreenshotsToJSONDelegate)
        {
            // ...
        }
    }
}