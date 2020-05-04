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
    public class DeleteApiProductsAsyncDelegate: DeleteAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate),
            typeof(ConvertApiProductToIndexDelegate),
            typeof(ConfirmApiProductsContainIdAsyncDelegate))]
        public DeleteApiProductsAsyncDelegate(
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