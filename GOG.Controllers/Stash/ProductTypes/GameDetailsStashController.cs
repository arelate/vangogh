using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Attributes;

using Controllers.Stash;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class GameDetailsStashController : StashController<List<GameDetails>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetGameDetailsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameDetailsStashController(
            IGetPathDelegate getGameDetailsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getGameDetailsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}