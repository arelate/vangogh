using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        PostListWishlistedRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetWishlistedRecordsPathDelegate))]
        public PostListWishlistedRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getWishlistedRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getWishlistedRecordsPathDelegate)
        {
            // ...
        }
    }
}