using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        PostListProductDownloadsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetProductDownloadsRecordsPathDelegate))]
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