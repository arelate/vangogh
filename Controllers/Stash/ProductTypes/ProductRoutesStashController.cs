using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductRoutes;

namespace Controllers.Stash.ProductTypes
{
    public class ProductRoutesStashController :
        StashController<List<ProductRoutes>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductRoutesStashController(
            IGetPathDelegate getProductRoutesPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductRoutesPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}