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
    public class ProductsDataController : DataController<Product>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListProductDataToPathAsyncDelegate,GOG.Delegates",  
            "GOG.Delegates.Convert.ProductTypes.ConvertProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Collections.ProductTypes.FindProductDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ProductsDataController(
            IGetDataAsyncDelegate<List<Product>> getListProductDataAsyncDelegate,
            IPostDataAsyncDelegate<List<Product>> postListProductDataAsyncDelegate,
            IConvertDelegate<Product, long> convertProductsToIndexDelegate,
            IRecordsController<long> productsRecordsIndexController,
            IFindDelegate<Product> findProductDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getListProductDataAsyncDelegate,
                postListProductDataAsyncDelegate,
                convertProductsToIndexDelegate,
                productsRecordsIndexController,
                findProductDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}