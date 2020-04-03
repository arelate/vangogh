using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListProductScreenshotsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.Records.PostListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetProductScreenshotsRecordsPathDelegate,Delegates")]
        public PostListProductScreenshotsRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getProductScreenshotsRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getProductScreenshotsRecordsPathDelegate)
        {
            // ...
        }
    }
}