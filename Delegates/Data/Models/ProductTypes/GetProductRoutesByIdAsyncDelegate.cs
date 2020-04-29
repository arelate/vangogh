using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetProductRoutesByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindProductRoutesDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates")]
        public GetProductRoutesByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRoutes>, string> getDataCollectionAsyncDelegate, 
            IFindDelegate<ProductRoutes> findDelegate, 
            IConvertDelegate<ProductRoutes, long> convertProductToIndexDelegate) : 
            base(
                getDataCollectionAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}