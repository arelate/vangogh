using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListGameDetailsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToListGameDetailsDelegate))]
        public GetListGameDetailsDataAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<GameDetails>> convertJSONToListGameDetailsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListGameDetailsDelegate)
        {
            // ...
        }
    }
}