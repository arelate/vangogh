using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateAccountProductsAsyncDelegate: UpdateDataAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(DeleteAccountProductsAsyncDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertAccountProductToIndexDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmAccountProductsContainIdAsyncDelegate),
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