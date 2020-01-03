using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ApiProductsDataController : DataController<ApiProduct>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Controllers.Stash.ProductTypes.ApiProductsStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ApiProductsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ApiProductsDataController(
            IStashController<List<ApiProduct>> apiProductsStashController,
            IConvertDelegate<ApiProduct, long> convertApiProductsToIndexDelegate,
            IRecordsController<long> apiProductsRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                apiProductsStashController,
                convertApiProductsToIndexDelegate,
                apiProductsRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}