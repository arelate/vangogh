using System.Collections.Generic;
using Interfaces.Controllers.Records;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;
using Controllers.Data;
using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameProductDataDataController : DataController<GameProductData>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameProductDataDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListGameProductDataDataToPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameProductDataToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.GameProductDataRecordsIndexController,Controllers",
            "GOG.Delegates.Collections.ProductTypes.FindGameProductDataDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public GameProductDataDataController(
            IGetDataAsyncDelegate<List<GameProductData>> getListGameProductDataDataAsyncDelegate,
            IPostDataAsyncDelegate<List<GameProductData>> postListGameProductDataDataAsyncDelegate,
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate,
            IRecordsController<long> gameProductDataRecordsIndexController,
            IFindDelegate<GameProductData> findGameProductDataDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getListGameProductDataDataAsyncDelegate,
                postListGameProductDataDataAsyncDelegate,
                convertGameProductDataToIndexDelegate,
                gameProductDataRecordsIndexController,
                findGameProductDataDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}