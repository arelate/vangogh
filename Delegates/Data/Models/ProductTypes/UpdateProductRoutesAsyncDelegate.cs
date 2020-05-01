using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateProductRoutesAsyncDelegate: UpdateDataAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            typeof(Delegates.Data.Models.ProductTypes.DeleteProductRoutesAsyncDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmProductRoutesContainIdAsyncDelegate),
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