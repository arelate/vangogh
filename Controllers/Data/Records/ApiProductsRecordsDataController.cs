using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class ApiProductsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Controllers.Stash.Records.ApiProductsRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ApiProductsRecordsDataController(
            IStashController<List<ProductRecords>> apiProductsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                apiProductsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}