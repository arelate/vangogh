using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductRoutesDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductRoutesDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate,Delegates")]
        public GetListProductRoutesDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRoutes>> getListProductRoutesDataAsyncDelegate, 
            IGetPathDelegate getProductRoutesPathDelegate) : 
            base(
                getListProductRoutesDataAsyncDelegate, 
                getProductRoutesPathDelegate)
        {
            // ...
        }
    }
}