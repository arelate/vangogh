using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListUpdatedDataAsyncDelegate : GetJSONDataAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.System.ConvertJSONToListLongDelegate))]
        public GetListUpdatedDataAsyncDelegate(
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