using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductRoutesDataController : DataController<ProductRoutes>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.ProductRoutesStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductRoutesRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProductRoutesDataController(
            IStashController<List<ProductRoutes>> productRoutesStashController,
            IConvertDelegate<ProductRoutes, long> convertProductRoutesToIndexDelegate,
            IRecordsController<long> productRoutesRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                productRoutesStashController,
                convertProductRoutesToIndexDelegate,
                productRoutesRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}