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
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertJSONToListProductRoutesDelegate,Delegates")]
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