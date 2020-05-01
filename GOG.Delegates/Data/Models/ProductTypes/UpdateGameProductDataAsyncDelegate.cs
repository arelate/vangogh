using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateGameProductDataAsyncDelegate: UpdateDataAsyncDelegate<GameProductData>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Models.ProductTypes.DeleteGameProductDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertGameProductDataToIndexDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmGameProductDataContainIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameProductDataDataFromPathAsyncDelegate))]
        public UpdateGameProductDataAsyncDelegate(
            IDeleteAsyncDelegate<GameProductData> deleteGameProductDataAsyncDelegate, 
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmGameProductDataContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<GameProductData>, string> getGameProductDataAsyncDelegate) : 
            base(
                deleteGameProductDataAsyncDelegate, 
                convertGameProductDataToIndexDelegate,
                confirmGameProductDataContainsIdAsyncDelegate, 
                getGameProductDataAsyncDelegate)
        {
            // ...
        }
    }
}