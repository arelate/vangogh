using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductRoutesDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.GetStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.ProductTypes.ConvertJSONToListProductRoutesDelegate))]
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