using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class PostListUpdatedRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetUpdatedRecordsPathDelegate))]
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