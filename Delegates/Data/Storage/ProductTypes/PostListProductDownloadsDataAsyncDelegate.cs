using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDownloadsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.Data.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertListProductDownloadsToJSONDelegate,Delegates")]        
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