using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductScreenshots;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ProductScreenshotsStashController :
        StashController<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
        public ProductScreenshotsStashController(
            IGetPathDelegate getProductScreenshotsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductScreenshotsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}