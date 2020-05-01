using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListGameProductDataDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.PostListGameProductDataDataAsyncDelegate),
            typeof(GetGameProductDataPathDelegate))]
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