using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListProductDelegate))]
        public GetListProductDataAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<Product>> convertJSONToListProductDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductDelegate)
        {
            // ...
        }
    }
}