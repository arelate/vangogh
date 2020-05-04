using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateApiProductsAsyncDelegate: UpdateDataAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(DeleteApiProductsAsyncDelegate),
            typeof(ConvertApiProductToIndexDelegate),
            typeof(ConfirmApiProductsContainIdAsyncDelegate),
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