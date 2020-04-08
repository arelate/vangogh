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
    public class ProductRoutesDataController : DataController<ProductRoutes>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListProductRoutesDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductRoutesRecordsIndexController,Controllers",
            "Delegates.Collections.ProductTypes.FindProductRoutesDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ProductRoutesDataController(
            IGetDataAsyncDelegate<List<ProductRoutes>> getProductRoutesDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRoutes>> postProductRoutesDataAsyncDelegate,
            IConvertDelegate<ProductRoutes, long> convertProductRoutesToIndexDelegate,
            IRecordsController<long> productRoutesRecordsIndexController,
            IFindDelegate<ProductRoutes> findProductRoutesDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductRoutesDataAsyncDelegate,
                postProductRoutesDataAsyncDelegate,
                convertProductRoutesToIndexDelegate,
                productRoutesRecordsIndexController,
                findProductRoutesDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}