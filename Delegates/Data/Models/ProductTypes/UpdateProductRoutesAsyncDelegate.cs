using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Confirmations.ProductTypes;
using Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateProductRoutesAsyncDelegate: UpdateDataAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            typeof(DeleteProductRoutesAsyncDelegate),
            typeof(ConvertProductRoutesToIndexDelegate),
            typeof(ConfirmProductRoutesContainIdAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate))]
        public UpdateProductRoutesAsyncDelegate(
            IDeleteAsyncDelegate<ProductRoutes> deleteAsyncDelegate, 
            IConvertDelegate<ProductRoutes, long> convertProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<ProductRoutes>, string> getListProductRoutesAsyncDelegate) : 
            base(
                deleteAsyncDelegate,
                convertProductToIndexDelegate, 
                confirmDataContainsIdAsyncDelegate, 
                getListProductRoutesAsyncDelegate)
        {
            // ...
        }
    }
}