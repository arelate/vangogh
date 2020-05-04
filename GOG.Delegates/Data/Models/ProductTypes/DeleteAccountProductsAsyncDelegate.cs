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
    public class DeleteAccountProductsAsyncDelegate: DeleteAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate),
            typeof(ConvertAccountProductToIndexDelegate),
            typeof(ConfirmAccountProductsContainIdAsyncDelegate))]
        public DeleteAccountProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>, string> getDataCollectionAsyncDelegate, 
            IConvertDelegate<AccountProduct, long> convertProductToIndexDelegate, 
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