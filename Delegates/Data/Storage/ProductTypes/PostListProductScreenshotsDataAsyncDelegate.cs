using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductScreenshotsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListProductScreenshotsToJSONDelegate))]
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