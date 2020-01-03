using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class UpdatedStashController : StashController<List<long>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdatedStashController(
            IGetPathDelegate getUpdatedPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getUpdatedPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}