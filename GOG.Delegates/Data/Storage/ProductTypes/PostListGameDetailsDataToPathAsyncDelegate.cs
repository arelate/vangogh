using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListGameDetailsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.PostListGameDetailsDataAsyncDelegate),
            typeof(GetGameDetailsPathDelegate))]
        public PostListGameDetailsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<GameDetails>> postListGameDetailsDataAsyncDelegate,
            IGetPathDelegate getGameDetailsPathDelegate) :
            base(
                postListGameDetailsDataAsyncDelegate,
                getGameDetailsPathDelegate)
        {
            // ...
        }
    }
}