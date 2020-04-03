using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListGameDetailsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.Records.PostListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetGameDetailsRecordsPathDelegate,Delegates")]
        public PostListGameDetailsRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getGameDetailsRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getGameDetailsRecordsPathDelegate)
        {
            // ...
        }
    }
}