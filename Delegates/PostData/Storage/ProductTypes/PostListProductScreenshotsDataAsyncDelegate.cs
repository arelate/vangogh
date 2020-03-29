using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.ProductTypes
{
    public class PostListProductScreenshotsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertListProductScreenshotsToJSONDelegate,Delegates")]        
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