using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Controllers.Stash;

using GOG.Models;

namespace GOG.Controllers.Stash.Data
{
    public class ProductsStashController :
        StashController<Dictionary<long, Product>>
    {
        public ProductsStashController(
            IGetPathDelegate getProductsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}