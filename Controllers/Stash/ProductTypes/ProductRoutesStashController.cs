using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.ProductRoutes;

namespace Controllers.Stash.ProductTypes
{
    public class ProductRoutesStashController :
        StashController<Dictionary<long, ProductRoutes>>
    {
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