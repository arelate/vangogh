using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListUpdatedDataAsyncDelegate : PostJSONDataAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.PostStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.System.ConvertListLongToJSONDelegate))]
        public PostListUpdatedDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<List<long>, string> convertListLongToJSONDelegate) :
            base(
                postStringDataAsyncDelegate,
                convertListLongToJSONDelegate)
        {
            // ...
        }
    }
}