using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class PostListProductsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetProductsRecordsPathDelegate))]
        public PostListProductsRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getProductsRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}