using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListWishlistedDataAsyncDelegate : GetJSONDataAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.System.ConvertJSONToListLongDelegate))]
        public GetListWishlistedDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<long>> convertJSONToListLongDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListLongDelegate)
        {
            // ...
        }
    }
}