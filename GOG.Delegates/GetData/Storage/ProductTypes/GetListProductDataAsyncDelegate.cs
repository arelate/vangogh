using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<Product>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListProductDelegate,GOG.Delegates")]
        public GetListProductDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<Product>> convertJSONToListProductDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductDelegate)
        {
            // ...
        }
    }
}