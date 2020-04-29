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
            "Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmProductRoutesContainIdAsyncDelegate,Delegates")]
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