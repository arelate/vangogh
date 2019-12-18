using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ProductsDataController : DataController<Product>
    {
        [Dependencies(
            "GOG.Controllers.Stash.ProductTypes.ProductsStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ProductsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProductsDataController(
            IStashController<List<Product>> productsStashController,
            IConvertDelegate<Product, long> convertProductsToIndexDelegate,
            IRecordsController<long> productsRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                productsStashController,
                convertProductsToIndexDelegate,
                productsRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}