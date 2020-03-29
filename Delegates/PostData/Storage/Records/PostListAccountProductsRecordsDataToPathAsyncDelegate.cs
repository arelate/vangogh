using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListAccountProductsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetAccountProductsRecordsPathDelegate,Delegates")]
        public PostListAccountProductsRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getAccountProductsRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getAccountProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}