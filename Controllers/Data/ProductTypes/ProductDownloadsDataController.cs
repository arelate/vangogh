using System.Collections.Generic;
using Interfaces.Controllers.Records;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductDownloadsDataController : DataController<ProductDownloads>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListProductDownloadsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductDownloadsRecordsIndexController,Controllers",
            "Delegates.Collections.ProductTypes.FindProductDownloadsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ProductDownloadsDataController(
            IGetDataAsyncDelegate<List<ProductDownloads>> getProductDownloadsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductDownloads>> postProductDownloadsDataAsyncDelegate,
            IConvertDelegate<ProductDownloads, long> convertProductDownloadsToIndexDelegate,
            IRecordsController<long> productDownloadsRecordsIndexController,
            IFindDelegate<ProductDownloads> productDownloadsFindDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductDownloadsDataAsyncDelegate,
                postProductDownloadsDataAsyncDelegate,
                convertProductDownloadsToIndexDelegate,
                productDownloadsRecordsIndexController,
                productDownloadsFindDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}