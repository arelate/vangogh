using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListGameProductDataDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            typeof(PostListGameProductDataDataAsyncDelegate),
            typeof(GetGameProductDataPathDelegate))]
        public PostListGameProductDataDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<GameProductData>> postListGameProductDataDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getGameProductDataPathDelegate) :
            base(
                postListGameProductDataDataAsyncDelegate,
                getGameProductDataPathDelegate)
        {
            // ...
        }
    }
}