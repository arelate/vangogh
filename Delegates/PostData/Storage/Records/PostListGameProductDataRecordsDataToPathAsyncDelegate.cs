using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListGameProductDataRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.Records.PostListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetGameProductDataRecordsPathDelegate,Delegates")]
        public PostListGameProductDataRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getGameProductDataRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getGameProductDataRecordsPathDelegate)
        {
            // ...
        }
    }
}