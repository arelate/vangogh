using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        PostListProductScreenshotsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetProductScreenshotsRecordsPathDelegate))]
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