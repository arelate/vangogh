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
    public class GetApiProductByIdAsyncDelegate: GetDataByIdAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate),
            typeof(FindApiProductDelegate),
            typeof(ConvertApiProductToIndexDelegate))]
        public GetApiProductByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<ApiProduct>, string> getListApiProductsAsyncDelegate, 
            IFindDelegate<ApiProduct> findDelegate, 
            IConvertDelegate<ApiProduct, long> convertApiProductToIndexDelegate) : 
            base(
                getListApiProductsAsyncDelegate, 
                findDelegate, 
                convertApiProductToIndexDelegate)
        {
            // ...
        }
    }
}