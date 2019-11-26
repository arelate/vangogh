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
    public class ApiProductsDataController : DataController<ApiProduct>
    {
        public ApiProductsDataController(
            IStashController<List<ApiProduct>> apiProductsStashController,
            IConvertDelegate<ApiProduct, long> convertApiProductsToIndexDelegate,
            IRecordsController<long> apiProductsRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                apiProductsStashController,
                convertApiProductsToIndexDelegate,
                apiProductsRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}