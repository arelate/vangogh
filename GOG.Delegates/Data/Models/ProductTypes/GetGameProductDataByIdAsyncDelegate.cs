using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Find.ProductTypes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetGameProductDataByIdAsyncDelegate: GetDataByIdAsyncDelegate<GameProductData>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameProductDataDataFromPathAsyncDelegate),
            typeof(FindGameProductDataDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertGameProductDataToIndexDelegate))]
        public GetGameProductDataByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<GameProductData>, string> getListGameProductDataAsyncDelegate, 
            IFindDelegate<GameProductData> findDelegate, 
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate) : 
            base(
                getListGameProductDataAsyncDelegate, 
                findDelegate, 
                convertGameProductDataToIndexDelegate)
        {
            // ...
        }
    }
}