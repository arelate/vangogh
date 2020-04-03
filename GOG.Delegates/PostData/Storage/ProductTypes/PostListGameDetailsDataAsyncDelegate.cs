using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListGameDetailsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertListGameDetailsToJSONDelegate,GOG.Delegates")]        
        public PostListGameDetailsDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<GameDetails>, string> convertListGameDetailsToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListGameDetailsToJSONDelegate)
        {
            // ...
        }
    }
}