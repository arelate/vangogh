using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateApiProductsAsyncDelegate: UpdateDataAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.DeleteApiProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmApiProductsContainIdAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate,GOG.Delegates")]
        public UpdateApiProductsAsyncDelegate(
            IDeleteAsyncDelegate<ApiProduct> deleteApiProductsAsyncDelegate, 
            IConvertDelegate<ApiProduct, long> convertApiProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmApiProductsContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<ApiProduct>, string> getApiProductsAsyncDelegate) : 
            base(
                deleteApiProductsAsyncDelegate, 
                convertApiProductToIndexDelegate,
                confirmApiProductsContainsIdAsyncDelegate, 
                getApiProductsAsyncDelegate)
        {
            // ...
        }
    }
}