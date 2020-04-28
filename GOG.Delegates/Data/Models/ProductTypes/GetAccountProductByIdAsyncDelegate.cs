using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetAccountProductByIdAsyncDelegate: GetDataByIdAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Collections.ProductTypes.FindAccountProductDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertAccountProductToIndexDelegate,GOG.Delegates")]
        public GetAccountProductByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>, string> getListAccountProductsAsyncDelegate, 
            IFindDelegate<AccountProduct> findDelegate, 
            IConvertDelegate<AccountProduct, long> convertProductToIndexDelegate) : 
            base(
                getListAccountProductsAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}