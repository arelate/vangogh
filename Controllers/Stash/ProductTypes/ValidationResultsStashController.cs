using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ValidationResultsStashController :
        StashController<List<ValidationResults>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.ProductTypes.GetValidationResultsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public ValidationResultsStashController(
            IGetPathDelegate getValidationResultsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getValidationResultsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}