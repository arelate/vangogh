using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Attributes;

using Controllers.Stash;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class ApiProductsStashController : StashController<List<ApiProduct>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetApiProductsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public ApiProductsStashController(
            IGetPathDelegate getApiProductsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getApiProductsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}