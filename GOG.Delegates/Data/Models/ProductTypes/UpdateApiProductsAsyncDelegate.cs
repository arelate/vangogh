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
            typeof(GOG.Delegates.Data.Models.ProductTypes.DeleteApiProductsAsyncDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmApiProductsContainIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate))]
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