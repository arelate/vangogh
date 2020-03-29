using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.ProductTypes
{
    public class PostListProductDownloadsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertListProductDownloadsToJSONDelegate,Delegates")]        
        public PostListProductDownloadsDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<ProductDownloads>, string> convertListProductDownloadsToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListProductDownloadsToJSONDelegate)
        {
            // ...
        }
    }
}