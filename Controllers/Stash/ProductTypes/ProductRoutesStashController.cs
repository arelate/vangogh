using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductRoutes;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ProductRoutesStashController :
        StashController<List<ProductRoutes>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
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