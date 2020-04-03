using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListGameDetailsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListGameDetailsDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetGameDetailsPathDelegate,Delegates")]
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