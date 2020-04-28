using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class DeleteProductsAsyncDelegate: DeleteAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmProductsContainIdAsyncDelegate.GOG.Delegates")]
        public DeleteProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<ApiProduct>, string> getDataCollectionAsyncDelegate, 
            IConvertDelegate<ApiProduct, long> convertProductToIndexDelegate, 
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