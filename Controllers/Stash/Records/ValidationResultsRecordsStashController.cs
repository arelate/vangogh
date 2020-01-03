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
    public class ValidationResultsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.Records.GetValidationResultsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public ValidationResultsRecordsStashController(
            IGetPathDelegate getValidationResultsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getValidationResultsRecordsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}