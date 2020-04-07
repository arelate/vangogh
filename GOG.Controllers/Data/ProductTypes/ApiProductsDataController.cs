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
    public class ApiProductsDataController : DataController<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListApiProductDataToPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ApiProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Collections.ProductTypes.FindApiProductDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ApiProductsDataController(
            IGetDataAsyncDelegate<List<ApiProduct>> getListApiProductDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ApiProduct>> postListApiProductDataAsyncDelegate,
            IConvertDelegate<ApiProduct, long> convertApiProductsToIndexDelegate,
            IRecordsController<long> apiProductsRecordsIndexController,
            IFindDelegate<ApiProduct> findApiProductDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getListApiProductDataAsyncDelegate,
                postListApiProductDataAsyncDelegate,
                convertApiProductsToIndexDelegate,
                apiProductsRecordsIndexController,
                findApiProductDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}