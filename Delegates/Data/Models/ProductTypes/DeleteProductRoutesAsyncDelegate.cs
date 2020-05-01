using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteProductRoutesAsyncDelegate : DeleteAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmProductRoutesContainIdAsyncDelegate))]
        public DeleteProductRoutesAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRoutes>, string> getDataCollectionAsyncDelegate,
            IConvertDelegate<ProductRoutes, long> convertProductToIndexDelegate,
            IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate) :
            base(
                getDataCollectionAsyncDelegate,
                convertProductToIndexDelegate,
                confirmContainsAsyncDelegate)
        {
            // ...
        }
    }
}