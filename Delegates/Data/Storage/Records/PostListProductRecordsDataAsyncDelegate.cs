using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.Records;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class PostListProductRecordsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListProductRecordsToJSONDelegate))]
        public PostListProductRecordsDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<List<ProductRecords>, string> convertListProductRecordsToJSONDelegate) :
            base(
                postStringDataAsyncDelegate,
                convertListProductRecordsToJSONDelegate)
        {
            // ...
        }
    }
}