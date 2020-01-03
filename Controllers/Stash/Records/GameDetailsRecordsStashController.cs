using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class GameDetailsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.Records.GetGameDetailsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameDetailsRecordsStashController(
            IGetPathDelegate getGameDetailsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getGameDetailsRecordsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}