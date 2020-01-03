using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductDownloadsDataController : DataController<ProductDownloads>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ProductTypes.ProductDownloadsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductDownloadsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductDownloadsDataController(
            IStashController<List<ProductDownloads>> productDownloadsStashController,
            IConvertDelegate<ProductDownloads, long> convertProductDownloadsToIndexDelegate,
            IRecordsController<long> productDownloadsRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                productDownloadsStashController,
                convertProductDownloadsToIndexDelegate,
                productDownloadsRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}