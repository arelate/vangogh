using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductDownloads;

namespace Controllers.Stash.ProductTypes
{
    public class ProductDownloadsStashController :
        StashController<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductDownloadsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductDownloadsStashController(
            IGetPathDelegate getProductDownloadsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductDownloadsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}