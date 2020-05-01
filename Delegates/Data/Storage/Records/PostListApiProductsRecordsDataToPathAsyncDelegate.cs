using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        PostListApiProductsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetApiProductsRecordsPathDelegate))]
        public PostListApiProductsRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getApiProductsRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getApiProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}