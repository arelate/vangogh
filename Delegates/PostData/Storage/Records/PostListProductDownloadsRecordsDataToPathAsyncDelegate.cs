using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListProductDownloadsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetProductDownloadsRecordsPathDelegate,Delegates")]
        public PostListProductDownloadsRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getProductDownloadsRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getProductDownloadsRecordsPathDelegate)
        {
            // ...
        }
    }
}