using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Controllers.Stash;

using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class ApiProductsStashController :
        StashController<Dictionary<long, ApiProduct>>
    {
        public ApiProductsStashController(
            IGetPathDelegate getApiProductsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getApiProductsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}