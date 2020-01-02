using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class SessionRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Controllers.Stash.Records.SessionRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public SessionRecordsDataController(
            IStashController<List<ProductRecords>> sessionRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                sessionRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}