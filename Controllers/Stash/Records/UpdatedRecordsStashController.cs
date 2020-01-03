using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class UpdatedRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.Records.GetUpdatedRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdatedRecordsStashController(
            IGetPathDelegate getUpdatedRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getUpdatedRecordsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}