using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ProductDownloads;

namespace Controllers.Data.ProductTypes
{
    public class ProductDownloadsDataController : DataController<ProductDownloads>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.ProductDownloadsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductDownloadsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductDownloadsDataController(
            IStashController<List<ProductDownloads>> productDownloadsStashController,
            IConvertDelegate<ProductDownloads, long> convertProductDownloadsToIndexDelegate,
            IRecordsController<long> productDownloadsRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                productDownloadsStashController,
                convertProductDownloadsToIndexDelegate,
                productDownloadsRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}