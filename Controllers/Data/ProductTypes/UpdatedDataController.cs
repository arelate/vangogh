using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;


using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class UpdatedDataController : DataController<long>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.UpdatedStashController,Controllers",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.UpdatedRecordsIndexController,Controllers",
            "Delegates.Find.System.FindLongDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdatedDataController(
            IStashController<List<long>> updatedDataController,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> updatedRecordsIndexController,
            IFindDelegate<long> findLongDelegate,
            IActionLogController actionLogController) :
            base(
                updatedDataController,
                convertPassthroughIndexDelegate,
                updatedRecordsIndexController,
                findLongDelegate,
                actionLogController)
        {
            // ...
        }
    }
}