using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        PostListProductRoutesRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetProductRoutesRecordsPathDelegate))]
        public PostListProductRoutesRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductRoutesRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getProductRoutesRecordsPathDelegate)
        {
            // ...
        }
    }
}