using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductRoutesDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertJSONToListProductRoutesDelegate,Delegates")]
        public GetListProductRoutesDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductRoutes>> convertJSONToListProductRoutesDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductRoutesDelegate)
        {
            // ...
        }
    }
}