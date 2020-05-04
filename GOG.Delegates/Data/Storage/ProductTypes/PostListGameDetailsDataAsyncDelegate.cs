using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListGameDetailsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListGameDetailsToJSONDelegate))]
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