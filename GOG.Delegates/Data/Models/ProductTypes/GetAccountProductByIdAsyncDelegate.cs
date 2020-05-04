using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Collections.ProductTypes;
using GOG.Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetAccountProductByIdAsyncDelegate: GetDataByIdAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate),
            typeof(FindAccountProductDelegate),
            typeof(ConvertAccountProductToIndexDelegate))]
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