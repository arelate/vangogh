using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

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
            "Controllers.Status.StatusController,Controllers")]
        public GameDetailsStashController(
            IGetPathDelegate getGameDetailsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getGameDetailsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}