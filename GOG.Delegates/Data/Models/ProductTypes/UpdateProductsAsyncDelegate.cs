using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateProductsAsyncDelegate: UpdateDataAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.DeleteProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmProductsContainIdAsyncDelegate.GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate,GOG.Delegates")]
        public UpdateProductsAsyncDelegate(
            IDeleteAsyncDelegate<ApiProduct> deleteProductsAsyncDelegate, 
            IConvertDelegate<ApiProduct, long> convertApiProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmProductsContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<ApiProduct>, string> getProductsAsyncDelegate) : 
            base(
                deleteProductsAsyncDelegate, 
                convertApiProductToIndexDelegate,
                confirmProductsContainsIdAsyncDelegate, 
                getProductsAsyncDelegate)
        {
            // ...
        }
    }
}