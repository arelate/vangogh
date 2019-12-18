using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ProductScreenshotsStashController :
        StashController<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProductScreenshotsStashController(
            IGetPathDelegate getProductScreenshotsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getProductScreenshotsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}