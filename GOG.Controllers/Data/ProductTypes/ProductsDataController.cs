using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ProductsDataController : DataController<Product>
    {
        public ProductsDataController(
            IStashController<Dictionary<long, Product>> productsStashController,
            IConvertDelegate<Product, long> convertProductsToIndexDelegate,
            IRecordsController<long> productsRecordsIndexController,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                productsStashController,
                convertProductsToIndexDelegate,
                productsRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}