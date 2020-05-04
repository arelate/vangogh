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
    public class UpdateAccountProductsAsyncDelegate: UpdateDataAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(DeleteAccountProductsAsyncDelegate),
            typeof(ConvertAccountProductToIndexDelegate),
            typeof(ConfirmAccountProductsContainIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate))]
        public UpdateAccountProductsAsyncDelegate(
            IDeleteAsyncDelegate<AccountProduct> deleteAccountProductsAsyncDelegate, 
            IConvertDelegate<AccountProduct, long> convertAccountProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmAccountProductsContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<AccountProduct>, string> getAccountProductsAsyncDelegate) : 
            base(
                deleteAccountProductsAsyncDelegate, 
                convertAccountProductToIndexDelegate,
                confirmAccountProductsContainsIdAsyncDelegate, 
                getAccountProductsAsyncDelegate)
        {
            // ...
        }
    }
}