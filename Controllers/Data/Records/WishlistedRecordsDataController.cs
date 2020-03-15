using System.Collections.Generic;

using Interfaces.Controllers.Stash;

using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class WishlistedRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.Records.WishlistedRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Find.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public WishlistedRecordsDataController(
            IStashController<List<ProductRecords>> wishlistedRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IActionLogController actionLogController) :
            base(
                wishlistedRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                findProductRecordsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}