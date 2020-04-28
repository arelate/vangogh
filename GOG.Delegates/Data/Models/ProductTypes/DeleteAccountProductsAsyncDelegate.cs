using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class DeleteAccountProductsAsyncDelegate: DeleteAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertAccountProductToIndexDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmAccountProductsContainIdAsyncDelegate.GOG.Delegates")]
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