using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ApiProductsDataController : DataController<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListApiProductDataToPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ApiProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindApiProductDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ApiProductsDataController(
            IGetDataAsyncDelegate<List<ApiProduct>> getListApiProductDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ApiProduct>> postListApiProductDataAsyncDelegate,
            IConvertDelegate<ApiProduct, long> convertApiProductsToIndexDelegate,
            IRecordsController<long> apiProductsRecordsIndexController,
            IFindDelegate<ApiProduct> findApiProductDelegate,
            IActionLogController actionLogController) :
            base(
                getListApiProductDataAsyncDelegate,
                postListApiProductDataAsyncDelegate,
                convertApiProductsToIndexDelegate,
                apiProductsRecordsIndexController,
                findApiProductDelegate,
                actionLogController)
        {
            // ...
        }
    }
}