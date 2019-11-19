using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Controllers.Stash;

using GOG.Models;

namespace GOG.Controllers.Stash.Data
{
    public class GameProductDataStashController :
        StashController<Dictionary<long, GameProductData>>
    {
        public GameProductDataStashController(
            IGetPathDelegate getGameProductDataPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getGameProductDataPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}