using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductRoutesDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToListProductRoutesDelegate))]
        public GetListProductRoutesDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductRoutes>> convertJSONToListProductRoutesDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductRoutesDelegate)
        {
            // ...
        }
    }
}