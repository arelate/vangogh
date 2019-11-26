using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Controllers.Stash;

using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class GameDetailsStashController : StashController<List<GameDetails>>
    {
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