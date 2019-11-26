using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ProductsDataController : DataController<Product>
    {
        public ProductsDataController(
            IStashController<List<Product>> productsStashController,
            IConvertDelegate<Product, long> convertProductsToIndexDelegate,
            IRecordsController<long> productsRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                productsStashController,
                convertProductsToIndexDelegate,
                productsRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}