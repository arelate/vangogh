using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.ProductDownloads;

namespace Controllers.Stash.ProductTypes
{
    public class ProductDownloadsStashController :
        StashController<Dictionary<long, ProductDownloads>>
    {
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