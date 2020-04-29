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
            "Delegates.Data.Models.ProductTypes.DeleteProductRoutesAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmProductRoutesContainIdAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate,Delegates")]
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