using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;


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
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductDownloadsStashController(
            IGetPathDelegate getProductDownloadsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getProductDownloadsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}