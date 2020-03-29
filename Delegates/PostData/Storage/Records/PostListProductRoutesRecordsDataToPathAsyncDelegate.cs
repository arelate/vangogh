using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListProductRoutesRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetProductRoutesRecordsPathDelegate,Delegates")]
        public PostListProductRoutesRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getProductRoutesRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getProductRoutesRecordsPathDelegate)
        {
            // ...
        }
    }
}