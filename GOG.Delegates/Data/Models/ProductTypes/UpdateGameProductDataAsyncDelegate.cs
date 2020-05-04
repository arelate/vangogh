using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateGameProductDataAsyncDelegate: UpdateDataAsyncDelegate<GameProductData>
    {
        [Dependencies(
            typeof(DeleteGameProductDataAsyncDelegate),
            typeof(ConvertGameProductDataToIndexDelegate),
            typeof(ConfirmGameProductDataContainIdAsyncDelegate),
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