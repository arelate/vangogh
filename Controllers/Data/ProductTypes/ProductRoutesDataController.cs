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
    public class ProductRoutesDataController : DataController<ProductRoutes>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.ProductTypes.PostListProductRoutesDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductRoutesRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindProductRoutesDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductRoutesDataController(
            IGetDataAsyncDelegate<List<ProductRoutes>> getProductRoutesDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRoutes>> postProductRoutesDataAsyncDelegate,
            IConvertDelegate<ProductRoutes, long> convertProductRoutesToIndexDelegate,
            IRecordsController<long> productRoutesRecordsIndexController,
            IFindDelegate<ProductRoutes> findProductRoutesDelegate,
            IActionLogController actionLogController) :
            base(
                getProductRoutesDataAsyncDelegate,
                postProductRoutesDataAsyncDelegate,
                convertProductRoutesToIndexDelegate,
                productRoutesRecordsIndexController,
                findProductRoutesDelegate,
                actionLogController)
        {
            // ...
        }
    }
}