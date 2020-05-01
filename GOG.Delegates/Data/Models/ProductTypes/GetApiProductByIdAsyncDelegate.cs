using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetApiProductByIdAsyncDelegate: GetDataByIdAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Collections.ProductTypes.FindApiProductDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates")]
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