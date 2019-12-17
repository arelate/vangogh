using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ProductDownloadsStashController :
        StashController<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetProductDownloadsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
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