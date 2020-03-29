using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListUpdatedRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetUpdatedRecordsPathDelegate,Delegates")]
        public PostListUpdatedRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getUpdatedRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getUpdatedRecordsPathDelegate)
        {
            // ...
        }
    }
}