using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListAccountProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListAccountProductDelegate))]
        public GetListAccountProductDataAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<AccountProduct>> convertJSONToListAccountProductDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListAccountProductDelegate)
        {
            // ...
        }
    }
}