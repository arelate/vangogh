using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductScreenshots;

namespace Controllers.Stash.ProductTypes
{
    public class ProductScreenshotsStashController :
        StashController<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
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