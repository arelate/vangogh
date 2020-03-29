using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListAccountProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.System.ConvertJSONToListAccountProductDelegate,GOG.Delegates")]
        public GetListAccountProductDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<AccountProduct>> convertJSONToListAccountProductDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListAccountProductDelegate)
        {
            // ...
        }
    }
}