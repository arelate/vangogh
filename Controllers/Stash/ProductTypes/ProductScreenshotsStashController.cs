using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.ProductScreenshots;

namespace Controllers.Stash.ProductTypes
{
    public class ProductScreenshotsStashController :
        StashController<List<ProductScreenshots>>
    {
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