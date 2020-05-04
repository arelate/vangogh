using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetProductRoutesByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate),
            typeof(Delegates.Collections.ProductTypes.FindProductRoutesDelegate),
            typeof(ConvertProductRoutesToIndexDelegate))]
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