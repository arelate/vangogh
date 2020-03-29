using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductDownloadsDataController : DataController<ProductDownloads>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.ProductTypes.PostListProductDownloadsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductDownloadsRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindProductDownloadsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductDownloadsDataController(
            IGetDataAsyncDelegate<List<ProductDownloads>> getProductDownloadsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductDownloads>> postProductDownloadsDataAsyncDelegate,
            IConvertDelegate<ProductDownloads, long> convertProductDownloadsToIndexDelegate,
            IRecordsController<long> productDownloadsRecordsIndexController,
            IFindDelegate<ProductDownloads> productDownloadsFindDelegate,
            IActionLogController actionLogController) :
            base(
                getProductDownloadsDataAsyncDelegate,
                postProductDownloadsDataAsyncDelegate,
                convertProductDownloadsToIndexDelegate,
                productDownloadsRecordsIndexController,
                productDownloadsFindDelegate,
                actionLogController)
        {
            // ...
        }
    }
}