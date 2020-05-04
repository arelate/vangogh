using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Find.ProductTypes;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetApiProductByIdAsyncDelegate: GetDataByIdAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate),
            typeof(FindApiProductDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate))]
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