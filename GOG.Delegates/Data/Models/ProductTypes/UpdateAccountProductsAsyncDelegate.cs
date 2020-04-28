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
            "GOG.Delegates.Data.Models.ProductTypes.DeleteAccountProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertAccountProductToIndexDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmAccountProductsContainIdAsyncDelegate.GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate,GOG.Delegates")]
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