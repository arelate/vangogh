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
    public class DeleteGameProductDataAsyncDelegate: DeleteAsyncDelegate<GameProductData>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameProductDataDataFromPathAsyncDelegate),
            typeof(ConvertGameProductDataToIndexDelegate),
            typeof(ConfirmGameProductDataContainIdAsyncDelegate))]
        public DeleteGameProductDataAsyncDelegate(
            IGetDataAsyncDelegate<List<GameProductData>, string> getDataCollectionAsyncDelegate, 
            IConvertDelegate<GameProductData, long> convertProductToIndexDelegate, 
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