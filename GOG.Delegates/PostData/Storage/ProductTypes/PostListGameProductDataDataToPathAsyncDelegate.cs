using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListGameProductDataDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostListGameProductDataDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetGameProductDataPathDelegate,Delegates")]
        public PostListGameProductDataDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<GameProductData>> postListGameProductDataDataAsyncDelegate,
            IGetPathDelegate getGameProductDataPathDelegate) :
            base(
                postListGameProductDataDataAsyncDelegate,
                getGameProductDataPathDelegate)
        {
            // ...
        }
    }
}