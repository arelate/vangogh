using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ValidationResultsStashController :
        StashController<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetValidationResultsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ResponseLogController,Controllers")]
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