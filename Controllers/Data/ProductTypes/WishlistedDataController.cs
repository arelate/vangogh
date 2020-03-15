using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class WishlistedDataController : DataController<long>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ProductTypes.WishlistedStashController,Controllers",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.WishlistedRecordsIndexController,Controllers",
            "Delegates.Find.System.FindLongDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public WishlistedDataController(
            IStashController<List<long>> wishlistedDataController,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> wishlistedRecordsIndexController,
            IFindDelegate<long> findLongDelegate,
            IActionLogController actionLogController) :
            base(
                wishlistedDataController,
                convertPassthroughIndexDelegate,
                wishlistedRecordsIndexController,
                findLongDelegate,
                actionLogController)
        {
            // ...
        }
    }
}