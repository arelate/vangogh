using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ProductRoutes;

namespace Controllers.Data.ProductTypes
{
    public class ProductRoutesDataController : DataController<ProductRoutes>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.ProductRoutesStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductRoutesRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductRoutesDataController(
            IStashController<List<ProductRoutes>> productRoutesStashController,
            IConvertDelegate<ProductRoutes, long> convertProductRoutesToIndexDelegate,
            IRecordsController<long> productRoutesRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                productRoutesStashController,
                convertProductRoutesToIndexDelegate,
                productRoutesRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}