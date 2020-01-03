using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Controllers.Stash;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class GameProductDataStashController : StashController<List<GameProductData>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.ProductTypes.GetGameProductDataPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameProductDataStashController(
            IGetPathDelegate getGameProductDataPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getGameProductDataPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}