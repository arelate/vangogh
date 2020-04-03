using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListApiProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertListApiProductToJSONDelegate,GOG.Delegates")]        
        public PostListApiProductDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<ApiProduct>, string> convertListApiProductToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListApiProductToJSONDelegate)
        {
            // ...
        }
    }
}