using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ProductsDataController : DataController<Product>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Controllers.Stash.ProductTypes.ProductsStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindProductDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductsDataController(
            IStashController<List<Product>> productsStashController,
            IConvertDelegate<Product, long> convertProductsToIndexDelegate,
            IRecordsController<long> productsRecordsIndexController,
            IFindDelegate<Product> findProductDelegate,
            IActionLogController actionLogController) :
            base(
                productsStashController,
                convertProductsToIndexDelegate,
                productsRecordsIndexController,
                findProductDelegate,
                actionLogController)
        {
            // ...
        }
    }
}