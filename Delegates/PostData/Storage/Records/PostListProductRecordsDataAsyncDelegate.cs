using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.Records
{
    public class PostListProductRecordsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
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