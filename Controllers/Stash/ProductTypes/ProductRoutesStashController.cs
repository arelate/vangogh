using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ProductRoutesStashController :
        StashController<List<ProductRoutes>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProductRoutesStashController(
            IGetPathDelegate getProductRoutesPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getProductRoutesPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}