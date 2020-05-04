using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToListProductDelegate))]
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