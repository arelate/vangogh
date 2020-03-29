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
    public class ProductsDataController : DataController<Product>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListProductDataToPathAsyncDelegate,GOG.Delegates",  
            "GOG.Delegates.Convert.ProductTypes.ConvertProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindProductDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductsDataController(
            IGetDataAsyncDelegate<List<Product>> getListProductDataAsyncDelegate,
            IPostDataAsyncDelegate<List<Product>> postListProductDataAsyncDelegate,
            IConvertDelegate<Product, long> convertProductsToIndexDelegate,
            IRecordsController<long> productsRecordsIndexController,
            IFindDelegate<Product> findProductDelegate,
            IActionLogController actionLogController) :
            base(
                getListProductDataAsyncDelegate,
                postListProductDataAsyncDelegate,
                convertProductsToIndexDelegate,
                productsRecordsIndexController,
                findProductDelegate,
                actionLogController)
        {
            // ...
        }
    }
}