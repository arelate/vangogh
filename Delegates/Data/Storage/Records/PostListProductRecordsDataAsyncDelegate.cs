using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class PostListProductRecordsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.Data.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.Records.ConvertListProductRecordsToJSONDelegate,Delegates")]        
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