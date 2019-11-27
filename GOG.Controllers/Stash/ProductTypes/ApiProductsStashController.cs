using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Controllers.Stash;

using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class ApiProductsStashController : StashController<List<ApiProduct>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetApiProductsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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